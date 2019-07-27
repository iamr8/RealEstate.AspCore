using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using EFSecondLevelCache.Core;
using EFSecondLevelCache.Core.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database;
using RealEstate.Services.Database.Base;
using RealEstate.Services.Extensions;
using RealEstate.Services.ViewModels;
using RealEstate.Services.ViewModels.Json;

namespace RealEstate.Services.ServiceLayer.Base
{
    public interface IBaseService
    {
        CurrentUserViewModel CurrentUser();

        Task<StatusEnum> SyncAsync<TSource, TModel>(ICollection<TSource> currentListEntities, List<TModel> newList,
            Func<TModel, CurrentUserViewModel, TSource> newEntity, Expression<Func<TSource, object>> duplicateCheck, Expression<Func<TSource, TModel, bool>> idValidator,
            Func<TSource, TModel, bool> valueValidator, Action<TSource, TModel> onUpdate, Role[] allowedRoles) where TSource : BaseEntity;

        Task<StatusEnum> RemoveAsync<TEntity>(TEntity entity, Role[] allowedRoles, DeleteEnum type = DeleteEnum.HideUnhide, bool save = true)
            where TEntity : BaseEntity;

        Task<PaginationViewModel<TOutput>> PaginateAsync<TQuery, TOutput>(IQueryable<TQuery> query, int page, string cacheKeysJson,
            Func<TQuery, TOutput> viewModel, int pageSize = 10, bool loadData = true)
            where TQuery : BaseEntity where TOutput : BaseLogViewModel;

        IQueryable<TSource> CheckDeletedItemsPrevillege<TSource, TSearch>(DbSet<TSource> source, TSearch searchModel, out CurrentUserViewModel currentUser) where TSource : BaseEntity where TSearch : BaseSearchModel;

        IQueryable<TSource> QueryByRole<TSource>(IQueryable<TSource> source, params Role[] allowedRolesToShowDeletedItems) where TSource : class;

        TModel Map<TSource, TModel>(TSource query,
            TModel entity) where TSource : class where TModel : class;

        IQueryable<TSource> AdminSeachConditions<TSource, TSearch>(IQueryable<TSource> query, TSearch searchModel, CurrentUserViewModel currentUser = null)
            where TSource : BaseEntity where TSearch : BaseSearchModel;

        Task<MethodStatus<TSource>> AddAsync<TSource>(Func<CurrentUserViewModel, TSource> entity,
            Role[] allowedRoles, bool save) where TSource : BaseEntity;

        Task<MethodStatus<TSource>> AddAsync<TSource>(DbSet<TSource> entities, Expression<Func<TSource, bool>> duplicateCondition, TSource entity,
            Role[] allowedRoles, bool save) where TSource : BaseEntity;

        Task<MethodStatus<TSource>> AddAsync<TSource>(TSource entity,
            Role[] allowedRoles, bool save) where TSource : BaseEntity;

        bool IsAllowed(params Role[] roles);

        Task<MethodStatus<TSource>> UpdateAsync<TSource>(TSource entity,
            Action<CurrentUserViewModel> changes, Role[] allowedRoles, bool save, StatusEnum modelNullStatus) where TSource : BaseEntity;

        Task<PaginationViewModel<TOutput>> PaginateAsync<TQuery, TOutput, TSearch>(IQueryable<TQuery> query, TSearch searchModel, Func<TQuery, TOutput> viewModel,
            int pageSize = 10, bool loadData = true)
            where TQuery : BaseEntity where TOutput : BaseLogViewModel where TSearch : BaseSearchModel;

        Task<StatusEnum> SaveChangesAsync();

        Task<MethodStatus<TModel>> SaveChangesAsync<TModel>(TModel model) where TModel : class;

        CurrentUserViewModel CurrentUser(List<Claim> claims);
    }

    public class BaseService : IBaseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _accessor;

        public BaseService(
            IUnitOfWork unitOfWork,
            IHttpContextAccessor accessor
            )
        {
            _unitOfWork = unitOfWork;
            _accessor = accessor;
        }

        public async Task<PaginationViewModel<TOutput>> PaginateAsync<TQuery, TOutput, TSearch>(IQueryable<TQuery> query, TSearch searchModel,
            Func<TQuery, TOutput> viewModel, int pageSize = 10, bool loadData = true)
            where TQuery : BaseEntity where TOutput : BaseLogViewModel where TSearch : BaseSearchModel
        {
            var efCacheKey = JsonConvert.SerializeObject(searchModel, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            var page = searchModel?.PageNo ?? 1;

            var result = await PaginateAsync(query, page, efCacheKey, viewModel, pageSize, loadData);
            return result;
        }

        public async Task<PaginationViewModel<TOutput>> PaginateAsync<TQuery, TOutput>(IQueryable<TQuery> query, int page, string cacheKeysJson,
          Func<TQuery, TOutput> viewModel, int pageSize = 10, bool loadData = true)
          where TQuery : BaseEntity where TOutput : BaseLogViewModel
        {
            var output = new PaginationViewModel<TOutput>();

            page = page <= 1 ? 1 : page;
            var efPolicy = new EFCachePolicy(CacheExpirationMode.Absolute, TimeSpan.FromDays(1), cacheKeysJson);
            var efDebug = new EFCacheDebugInfo();

            query = query.OrderDescendingByCreationDateTime();
            var rowCount = await query
                .Cacheable(efPolicy, efDebug)
                .CountAsync();
            if (rowCount <= 0)
                return output;

            var pageCount = NumberProcessorExtensions.RoundToUp((double)rowCount / pageSize);
            if (page > pageCount)
                page = 1;

            var viewList = new List<TOutput>();
            if (loadData)
            {
                var pagingQuery = page > 1
                    ? query.Skip(pageSize * (page - 1))
                    : query;
                pagingQuery = pagingQuery.Take(pageSize);

                var entities = await pagingQuery
                    .Cacheable(efPolicy, efDebug)
                    .ToListAsync();
                if (entities?.Any() != true)
                    return output;

                viewList = entities
                    .Select(viewModel.Invoke)
                    .ToHasNotNullList();

                if (viewList == null)
                    return output;
            }

            output.Items = viewList;
            output.CurrentPage = page;
            output.Pages = NumberProcessorExtensions.RoundToUp((double)rowCount / pageSize);
            output.Rows = rowCount;
            return output;
        }

        private HttpContext HttpContext => _accessor.HttpContext;

        public IQueryable<TSource> AdminSeachConditions<TSource, TSearch>(IQueryable<TSource> query, TSearch searchModel, CurrentUserViewModel currentUser = null) where TSource : BaseEntity where TSearch : BaseSearchModel
        {
            currentUser = currentUser ?? CurrentUser();
            if (currentUser == null)
                return query;

            if (searchModel == null)
                return query;

            if (!string.IsNullOrEmpty(searchModel.CreationDateFrom))
            {
                var dateTime = searchModel.CreationDateFrom.PersianToGregorian().ToString("yyyy/MM/dd", new CultureInfo("en-US"));
                query = query.Where(x => CustomDbFunctions.DateDiff("DAY", dateTime, CustomDbFunctions.JsonValue(x.Audit, "$[0].d")) <= 0);
            }

            if (!string.IsNullOrEmpty(searchModel.CreationDateTo))
            {
                var dateTime = searchModel.CreationDateTo.PersianToGregorian().ToString("yyyy/MM/dd", new CultureInfo("en-US"));
                query = query.Where(x => CustomDbFunctions.DateDiff("DAY", dateTime, CustomDbFunctions.JsonValue(x.Audit, "$[0].d")) >= 0);
            }

            if (currentUser.Role == Role.SuperAdmin || currentUser.Role == Role.Admin)
            {
                if (string.IsNullOrEmpty(searchModel.CreatorId))
                    return query;

                query = query.Where(entity => CustomDbFunctions.JsonValue(entity.Audit, "$[0].i") == searchModel.CreatorId);
            }

            return query;
        }

        public IQueryable<TSource> CheckDeletedItemsPrevillege<TSource, TSearch>(DbSet<TSource> source, TSearch searchModel, out CurrentUserViewModel currentUser) where TSource : BaseEntity where TSearch : BaseSearchModel
        {
            currentUser = CurrentUser();
            if (currentUser == null)
                return null;

            var isAdmin = searchModel?.IncludeDeletedItems == true && (currentUser.Role == Role.Admin || currentUser.Role == Role.SuperAdmin);
            var query = source
                .AsQueryable()
                .AsNoTracking();
            var tableName = query.ElementType.Name;

            var rawQuery =
                $"SELECT * FROM [{tableName}] I CROSS APPLY ( SELECT TOP(1) JSON_VALUE(value, '$.d') ActivityDate, JSON_VALUE(value, '$.t') ActivityType FROM OPENJSON(I.Audit, '$') J ORDER BY [key] DESC ) J2 ";

            if (!isAdmin)
                rawQuery += $"WHERE ActivityType != {(int)LogTypeEnum.Delete}";

            //else
            //{
            //            query = query.IgnoreQueryFilters();
            //}

            query = query.FromSql(rawQuery);
            return query;
        }

        public CurrentUserViewModel CurrentUser()
        {
            var context = HttpContext;
            var users = context?.User;
            if (users == null) return null;

            var claims = users.Claims.ToList();
            return CurrentUser(claims);
        }

        public async Task<MethodStatus<TSource>> AddAsync<TSource>(DbSet<TSource> entities, Expression<Func<TSource, bool>> duplicateCondition, TSource entity,
            Role[] allowedRoles, bool save) where TSource : BaseEntity
        {
            var duplicate = await entities.FirstOrDefaultAsync(duplicateCondition);
            if (duplicate != null)
                return new MethodStatus<TSource>(StatusEnum.AlreadyExists, null);

            return await AddAsync(entity, allowedRoles, save);
        }

        public async Task<MethodStatus<TSource>> AddAsync<TSource>(Func<CurrentUserViewModel, TSource> entity,
            Role[] allowedRoles, bool save) where TSource : BaseEntity
        {
            var currentUser = CurrentUser();
            if (currentUser == null)
                return new MethodStatus<TSource>(StatusEnum.UserIsNull, null);

            if (!IsAllowed(allowedRoles))
                return new MethodStatus<TSource>(StatusEnum.ForbiddenAndUnableToUpdateOrShow, null);

            var finalEntity = entity.Invoke(currentUser);
            _unitOfWork.Add(finalEntity, currentUser);
            return await SaveChangesAsync(finalEntity);
        }

        public async Task<MethodStatus<TSource>> AddAsync<TSource>(TSource entity,
            Role[] allowedRoles, bool save) where TSource : BaseEntity
        {
            var currentUser = CurrentUser();
            if (currentUser == null)
                return new MethodStatus<TSource>(StatusEnum.UserIsNull);

            if (!IsAllowed(allowedRoles))
                return new MethodStatus<TSource>(StatusEnum.ForbiddenAndUnableToUpdateOrShow);

            _unitOfWork.Add(entity, currentUser);

            if (save)
                return await SaveChangesAsync(entity);

            return new MethodStatus<TSource>(StatusEnum.Success);
        }

        public async Task<StatusEnum> RemoveAsync<TEntity>(TEntity entity, Role[] allowedRoles, DeleteEnum type = DeleteEnum.HideUnhide, bool save = true)
            where TEntity : BaseEntity
        {
            var currentUser = CurrentUser();
            if (currentUser == null)
                return StatusEnum.UserIsNull;

            if (!IsAllowed(allowedRoles))
                return StatusEnum.Forbidden;

            if (entity == null)
                return StatusEnum.ModelIsNull;

            if (type == DeleteEnum.Delete)
            {
                _unitOfWork.Delete(entity);
            }
            else
            {
                if (entity.LastAudit?.Type == LogTypeEnum.Delete)
                {
                    if (type == DeleteEnum.HideUnhide)
                        _unitOfWork.UnDelete(entity, currentUser);
                    else
                        return StatusEnum.AlreadyDeleted;
                }
                else
                {
                    _unitOfWork.Delete(entity, currentUser);
                }
            }

            return await SaveChangesAsync();
        }

        public TModel Map<TSource, TModel>(TSource query,
            TModel entity) where TSource : class where TModel : class
        {
            if (query == null)
                return null;

            var propertyEntityId = query.GetType().GetProperty("Id");
            if (propertyEntityId == null || !(propertyEntityId.GetValue(query) is string id))
                return entity;

            BaseViewModel templateBaseView;
            var propertyViewId = entity.GetType().GetProperty(nameof(templateBaseView.Id));
            if (propertyViewId != null && propertyViewId.PropertyType == typeof(string))
                propertyViewId.SetValue(entity, id);

            return entity;
        }

        public async Task<StatusEnum> SyncAsync<TSource, TModel>(ICollection<TSource> currentListEntities, List<TModel> newList,
            Func<TModel, CurrentUserViewModel, TSource> newEntity, Expression<Func<TSource, object>> duplicateCheck, Expression<Func<TSource, TModel, bool>> idValidator,
            Func<TSource, TModel, bool> valueValidator, Action<TSource, TModel> onUpdate, Role[] allowedRoles) where TSource : BaseEntity
        {
            var currentUser = CurrentUser();
            if (currentUser == null)
                return StatusEnum.UserIsNull;

            if (currentListEntities?.Any() == true)
            {
                var mustBeLeft = new List<TSource>();
                if (newList?.Any() == true)
                    mustBeLeft = currentListEntities.Where(entity => newList.Any(model => idValidator.Compile().Invoke(entity, model))).ToList();

                var duplicates = mustBeLeft.GroupBy(x => duplicateCheck.Compile().Invoke(x)).Where(x => x.Count() > 1).Select(x => x.FirstOrDefault()).ToList();
                var mustBeRemoved = currentListEntities.Where(x => !mustBeLeft.Contains(x)).ToList();
                mustBeRemoved.AddRange(duplicates);

                if (mustBeRemoved.Count > 0)
                    foreach (var redundant in mustBeRemoved)
                        await RemoveAsync(redundant, null, DeleteEnum.Delete, false);
            }

            if (newList?.Any() != true)
                return StatusEnum.Success;

            var changesIndicator = 0;
            foreach (var model in newList)
            {
                var source = currentListEntities?.FirstOrDefault(ent => idValidator.Compile().Invoke(ent, model));
                if (source == null)
                {
                    var newItem = newEntity.Invoke(model, currentUser);
                    await AddAsync(newItem, allowedRoles, false);
                    changesIndicator++;
                }
                else
                {
                    var noNeedChanges = valueValidator?.Invoke(source, model) == true;
                    if (noNeedChanges)
                        continue;

                    if (onUpdate == null)
                        continue;

                    onUpdate.Invoke(source, model);
                    _unitOfWork.Update(source, currentUser);
                    changesIndicator++;
                }
            }

            if (changesIndicator > 0)
                return await SaveChangesAsync();

            return StatusEnum.Success;
        }

        //public async Task<StatusEnum> SyncAsync<TSource, TModel>(ICollection<TSource> currentListEntities,
        //    List<TModel> newList, Func<TModel, CurrentUserViewModel, TSource> newEntity, Expression<Func<TSource, TModel, bool>> indentifier, Role[] allowedRoles, bool save) where TSource : BaseEntity
        //{
        //    var currentUser = CurrentUser();
        //    if (currentUser == null)
        //        return StatusEnum.UserIsNull;

        //    if (currentListEntities?.Any() == true)
        //    {
        //        var mustBeLeft = currentListEntities.Where(source => newList.Any(model => indentifier.Compile().Invoke(source, model))).ToList();
        //        var mustBeRemoved = currentListEntities.Where(x => !mustBeLeft.Contains(x)).ToList();
        //        if (mustBeRemoved.Count > 0)
        //            foreach (var redundant in mustBeRemoved)
        //                await RemoveAsync(redundant, null, DeleteEnum.Delete, false);
        //    }

        //    if (newList?.Any() != true)
        //        return await SaveChangesAsync();

        //    foreach (var model in newList)
        //    {
        //        var source = currentListEntities?.FirstOrDefault(entity => indentifier.Compile().Invoke(entity, model));
        //        if (source == null)
        //        {
        //            var newItem = newEntity.Invoke(model, currentUser);
        //            await AddAsync(newItem, allowedRoles, false);
        //        }
        //        else
        //        {
        //            if (!source.IsDeleted)
        //                continue;

        //            _unitOfWork.UnDelete(source, currentUser);
        //        }
        //    }

        //    return await SaveChangesAsync();
        //}

        public CurrentUserViewModel CurrentUser(List<Claim> claims)
        {
            if (claims.Count == 0) return null;
            var result = new CurrentUserViewModel
            {
                Username = claims.Find(x => x.Type == ClaimTypes.Name)?.Value,
                Mobile = claims.Find(x => x.Type == ClaimTypes.MobilePhone)?.Value,
                Role = claims.Find(x => x.Type == ClaimTypes.Role)?.Value.To<Role>() ?? Role.User,
                EncryptedPassword = claims.Find(x => x.Type == ClaimTypes.Hash)?.Value,
                Id = claims.Find(x => x.Type == ClaimTypes.NameIdentifier)?.Value,
                FirstName = claims.Find(x => x.Type == "FirstName")?.Value,
                LastName = claims.Find(x => x.Type == "LastName")?.Value,
                EmployeeId = claims.Find(x => x.Type == "EmployeeId")?.Value,
                UserPropertyCategories = claims.Find(x => x.Type == "PropertyCategories")?.Value.JsonConversion<List<CategoryJsonViewModel>>(),
                UserItemCategories = claims.Find(x => x.Type == "ItemCategories")?.Value.JsonConversion<List<CategoryJsonViewModel>>(),
                EmployeeDivisions = claims.Find(x => x.Type == "EmployeeDivisions")?.Value.JsonConversion<List<DivisionJsonViewModel>>(),
            };
            return result;
        }

        public IQueryable<TSource> QueryByRole<TSource>(IQueryable<TSource> source, params Role[] allowedRolesToShowDeletedItems) where TSource : class
        {
            var query = source;
            if (IsAllowed(allowedRolesToShowDeletedItems))
                query = query.IgnoreQueryFilters();

            return query;
        }

        public bool IsAllowed(params Role[] roles)
        {
            var currentUser = CurrentUser();
            if (currentUser == null)
                return false;

            return roles?.Any() != true || roles.Any(x => x == currentUser.Role);
        }

        public async Task<MethodStatus<TSource>> UpdateAsync<TSource>(TSource entity,
         Action<CurrentUserViewModel> changes, Role[] allowedRoles, bool save, StatusEnum modelNullStatus) where TSource : BaseEntity
        {
            var currentUser = CurrentUser();
            if (currentUser == null)
                return new MethodStatus<TSource>(StatusEnum.UserIsNull);

            if (!IsAllowed(allowedRoles))
                return new MethodStatus<TSource>(StatusEnum.Forbidden);

            if (entity == null)
                return new MethodStatus<TSource>(modelNullStatus);

            var propertiesBeforeChanges = entity.GetProperties();
            changes.Invoke(currentUser);
            var propertiesAfterChanges = entity.GetProperties();
            if (propertiesAfterChanges?.Any() != true)
                return new MethodStatus<TSource>(StatusEnum.Success, entity);

            var changesList = new Dictionary<string, string>();
            foreach (var (name, value) in propertiesAfterChanges)
            {
                var oldValue = propertiesBeforeChanges.FirstOrDefault(x => x.Key.Equals(name, StringComparison.CurrentCulture)).Value;
                var newValue = value;

                if (oldValue == null && newValue != null)
                    changesList.Add(name, null);
                else if (oldValue != null && !oldValue.Equals(newValue))
                    changesList.Add(name, oldValue.ToString());
            }

            if (changesList.Any() != true)
                return new MethodStatus<TSource>(StatusEnum.NoNeedToSave, entity);

            _unitOfWork.Update(entity, currentUser, changesList);

            if (save)
                return await SaveChangesAsync(entity);

            return new MethodStatus<TSource>(StatusEnum.Success, entity);
        }

        public async Task<StatusEnum> SaveChangesAsync()
        {
            var saveStatus = await _unitOfWork.SaveChangesAsync() > 0;
            return saveStatus
                ? StatusEnum.Success
                : StatusEnum.UnableToSave;
        }

        public async Task<MethodStatus<TModel>> SaveChangesAsync<TModel>(TModel model) where TModel : class
        {
            var saveStatus = await SaveChangesAsync();
            return new MethodStatus<TModel>(saveStatus, model);
        }
    }
}