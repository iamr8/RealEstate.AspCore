using EFSecondLevelCache.Core;
using EFSecondLevelCache.Core.Contracts;
using GeoAPI.Geometries;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MoreLinq;
using Newtonsoft.Json;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database;
using RealEstate.Services.Database.Base;
using RealEstate.Services.Extensions;
using RealEstate.Services.ViewModels;
using RealEstate.Services.ViewModels.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RealEstate.Services.ServiceLayer.Base
{
    public interface IBaseService
    {
        CurrentUserViewModel CurrentUser();

        Task<StatusEnum> SyncAsync<TSource, TModel>(ICollection<TSource> currentListEntities,
            List<TModel> newList, Func<TModel, TSource> newEntity, Expression<Func<TSource, TModel, bool>> indentifier, Func<TSource, TModel, bool> validator,
            Action<TSource, TModel> onUpdate,
            Role[] allowedRoles, bool save) where TSource : BaseEntity;

        Task<StatusEnum> RemoveAsync<TEntity>(TEntity entity, Role[] allowedRoles, DeleteEnum type = DeleteEnum.HideUnhide, bool save = true)
            where TEntity : BaseEntity;

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
            Task<bool> hasDuplicate, CurrentUserViewModel currentUser = null)
            where TQuery : BaseEntity where TOutput : BaseLogViewModel where TSearch : BaseSearchModel;

        Task<StatusEnum> SaveChangesAsync();

        Task<StatusEnum> SyncAsync<TSource, TModel>(ICollection<TSource> currentListEntities,
            List<TModel> newList, Func<TModel, CurrentUserViewModel, TSource> newEntity, Expression<Func<TSource, TModel, bool>> indentifier, Role[] allowedRoles,
            bool save)
            where TSource : BaseEntity;

        Task<MethodStatus<TModel>> SaveChangesAsync<TModel>(TModel model, bool save) where TModel : class;

        CurrentUserViewModel CurrentUser(List<Claim> claims);
    }

    public class BaseService : IBaseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _accessor;
        private readonly ApplicationDbContext _database;
        private readonly IServiceProvider _serviceProvider;

        public BaseService(
            IUnitOfWork unitOfWork,
            IHttpContextAccessor accessor,
            IServiceProvider serviceProvider,
            ApplicationDbContext database
            )
        {
            _unitOfWork = unitOfWork;
            _accessor = accessor;
            _database = database;
            _serviceProvider = serviceProvider;
        }

        public async Task<PaginationViewModel<TOutput>> PaginateAsync<TQuery, TOutput, TSearch>(IQueryable<TQuery> query, TSearch searchModel, Func<TQuery, TOutput> viewModel, Task<bool> hasDuplicate, CurrentUserViewModel currentUser = null)
            where TQuery : BaseEntity where TOutput : BaseLogViewModel where TSearch : BaseSearchModel
        {
            var output = new PaginationViewModel<TOutput>();

            currentUser = currentUser ?? CurrentUser();
            if (currentUser == null)
                return output;

            var page = searchModel?.PageNo ?? 1;
            page = page <= 1 ? 1 : page;
            const int pageSize = 10;

            var efCacheKey = JsonConvert.SerializeObject(searchModel, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            var efPolicy = new EFCachePolicy(CacheExpirationMode.Absolute, TimeSpan.FromDays(1), efCacheKey);
            var efDebug = new EFCacheDebugInfo();

            query = query.OrderDescendingByCreationDateTime();
            var rowCount = await query
                .Cacheable(efPolicy, efDebug)
                .CountAsync();
            if (rowCount <= 0)
                return output;

            var pageCount = NumberProcessorExtensions.RoundToUp((double)rowCount / pageSize);
            if (page > pageCount)
                page = pageCount;

            var pagingQuery = page > 1
            ? query.Skip(pageSize * (page - 1))
            : query;
            pagingQuery = pagingQuery.Take(pageSize);

            var entities = await pagingQuery
                .Cacheable(efPolicy, efDebug)
                .ToListAsync();
            if (entities?.Any() != true)
                return output;

            var viewList = entities
                .Select(viewModel.Invoke)
                //.OrderByDescending(x => x.Logs.Create.DateTime)
                .ToHasNotNullList();

            if (viewList == null)
                return output;

            output.CurrentPage = page;
            output.Pages = NumberProcessorExtensions.RoundToUp((double)rowCount / pageSize);
            output.Items = viewList;
            output.HasDuplicates = await hasDuplicate;
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
                var dtFrom = searchModel.CreationDateFrom.PersianToGregorian();
                query = query.Where(x => x.Audits.Find(c => c.Type == LogTypeEnum.Create).DateTime.Date >= dtFrom.Date);
            }

            if (!string.IsNullOrEmpty(searchModel.CreationDateTo))
            {
                var dtTo = searchModel.CreationDateTo.PersianToGregorian();
                query = query.Where(x => x.Audits.Find(c => c.Type == LogTypeEnum.Create).DateTime.Date <= dtTo.Date);
            }

            if (currentUser.Role == Role.SuperAdmin || currentUser.Role == Role.Admin)
            {
                if (string.IsNullOrEmpty(searchModel.CreatorId))
                    return query;

                query = query.Where(x => x.Audits.Find(c => c.Type == LogTypeEnum.Create).UserId == searchModel.CreatorId);
            }
            else
            {
                if (string.IsNullOrEmpty(searchModel.CreatorId))
                    return query;

                query = query.Where(x => x.Audits.Find(c => c.Type == LogTypeEnum.Create).UserId == currentUser.Id);
            }

            return query;
        }

        public IQueryable<TSource> CheckDeletedItemsPrevillege<TSource, TSearch>(DbSet<TSource> source, TSearch searchModel, out CurrentUserViewModel currentUser) where TSource : BaseEntity where TSearch : BaseSearchModel
        {
            currentUser = CurrentUser();
            if (currentUser == null)
                return null;

            var isAdmin = searchModel?.IncludeDeletedItems == true && (currentUser.Role == Role.Admin || currentUser.Role == Role.SuperAdmin);
            var query = source.AsQueryable();
            var dbServices = _database.GetService<IDbContextServices>();
            var entityType = dbServices.Model.FindEntityType(query.ElementType);
            var tableName = entityType.Relational().TableName;

            var rawQuery =
                $"SELECT * FROM [{tableName}] I CROSS APPLY ( SELECT TOP(1) JSON_VALUE(value, '$.d') ActivityDate, JSON_VALUE(value, '$.t') ActivityType FROM OPENJSON(I.Audit, '$') J ORDER BY [key] DESC ) J2 ";

            if (!isAdmin)
                rawQuery += $"WHERE ActivityType != {(int)LogTypeEnum.Delete}";

            query = query.IgnoreQueryFilters().FromSql(rawQuery);
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
            return await SaveChangesAsync(finalEntity, save);
        }

        public async Task<MethodStatus<TSource>> AddAsync<TSource>(TSource entity,
            Role[] allowedRoles, bool save) where TSource : BaseEntity
        {
            var currentUser = CurrentUser();
            if (currentUser == null)
                return new MethodStatus<TSource>(StatusEnum.UserIsNull, null);

            if (!IsAllowed(allowedRoles))
                return new MethodStatus<TSource>(StatusEnum.ForbiddenAndUnableToUpdateOrShow, null);

            _unitOfWork.Add(entity, currentUser);
            return await SaveChangesAsync(entity, save);
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

            var templateBaseView = new BaseViewModel();
            var propertyViewId = entity.GetType().GetProperty(nameof(templateBaseView.Id));
            if (propertyViewId != null && propertyViewId.PropertyType == typeof(string))
                propertyViewId.SetValue(entity, id);

            return entity;
        }

        public async Task<StatusEnum> SyncAsync<TSource, TModel>(ICollection<TSource> currentListEntities,
            List<TModel> newList, Func<TModel, TSource> newEntity, Expression<Func<TSource, TModel, bool>> indentifier, Func<TSource, TModel, bool> validator, Action<TSource, TModel> onUpdate, Role[] allowedRoles, bool save) where TSource : BaseEntity
        {
            var currentUser = CurrentUser();
            if (currentUser == null)
                return StatusEnum.UserIsNull;

            if (currentListEntities?.Any() == true)
            {
                var mustBeLeft = new List<TSource>();
                if (newList?.Any() == true)
                    mustBeLeft = currentListEntities.Where(entity => newList.Any(model => indentifier.Compile().Invoke(entity, model))).ToList();

                var mustBeRemoved = currentListEntities.Where(x => !mustBeLeft.Contains(x)).ToList();
                if (mustBeRemoved.Count > 0)
                    foreach (var redundant in mustBeRemoved)
                        await RemoveAsync(redundant, null, DeleteEnum.Delete, false);
            }

            if (newList?.Any() != true)
                return await SaveChangesAsync();

            foreach (var model in newList)
            {
                var source = currentListEntities?.FirstOrDefault(ent => indentifier.Compile().Invoke(ent, model));
                if (source == null)
                {
                    var newItem = newEntity.Invoke(model);
                    await AddAsync(newItem, allowedRoles, false);
                }
                else
                {
                    var noNeedChanges = validator?.Invoke(source, model) == true;
                    if (noNeedChanges)
                        continue;

                    onUpdate.Invoke(source, model);
                    _unitOfWork.Update(source, currentUser);
                }
            }

            return await SaveChangesAsync();
        }

        public async Task<StatusEnum> SyncAsync<TSource, TModel>(ICollection<TSource> currentListEntities,
            List<TModel> newList, Func<TModel, CurrentUserViewModel, TSource> newEntity, Expression<Func<TSource, TModel, bool>> indentifier, Role[] allowedRoles, bool save) where TSource : BaseEntity
        {
            var currentUser = CurrentUser();
            if (currentUser == null)
                return StatusEnum.UserIsNull;

            if (currentListEntities?.Any() == true)
            {
                var mustBeLeft = currentListEntities.Where(source => newList.Any(model => indentifier.Compile().Invoke(source, model))).ToList();
                var mustBeRemoved = currentListEntities.Where(x => !mustBeLeft.Contains(x)).ToList();
                if (mustBeRemoved.Count > 0)
                    foreach (var redundant in mustBeRemoved)
                        await RemoveAsync(redundant, null, DeleteEnum.Delete, false);
            }

            if (newList?.Any() != true)
                return await SaveChangesAsync();

            foreach (var model in newList)
            {
                var source = currentListEntities?.FirstOrDefault(entity => indentifier.Compile().Invoke(entity, model));
                if (source == null)
                {
                    var newItem = newEntity.Invoke(model, currentUser);
                    await AddAsync(newItem, allowedRoles, false);
                }
                else
                {
                    if (!source.IsDeleted)
                        continue;

                    _unitOfWork.UnDelete(source, currentUser);
                }
            }

            return await SaveChangesAsync();
        }

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
                return new MethodStatus<TSource>(StatusEnum.UserIsNull, null);

            if (!IsAllowed(allowedRoles))
                return new MethodStatus<TSource>(StatusEnum.Forbidden, null);

            if (entity == null)
                return new MethodStatus<TSource>(modelNullStatus, null);

            var oldEntity = entity.GetPublicProperties().Select(x => new
            {
                x.Name,
                Value = x.GetValue(entity)
            }).ToList();
            changes.Invoke(currentUser);
            var properties = entity.GetPublicProperties().Where(x => (x.PropertyType == typeof(string)
                                                                     || x.PropertyType == typeof(int)
                                                                     || x.PropertyType == typeof(decimal)
                                                                     || x.PropertyType == typeof(double)
                                                                     || x.PropertyType == typeof(IPoint)
                                                                     || x.PropertyType == typeof(DateTime)
                                                                     || x.PropertyType == typeof(Enum))
                                                                     && x.Name != nameof(entity.Id)
                                                                     && x.Name != nameof(entity.Audit)).ToList();
            if (properties?.Any() != true)
                return new MethodStatus<TSource>(StatusEnum.Success, entity);

            var changesList = new Dictionary<string, string>();
            foreach (var property in properties)
            {
                var name = property.Name;
                var oldValue = oldEntity.Find(x => x.Name.Equals(name)).Value;
                var newValue = property.GetValue(entity);

                if (oldValue == null && newValue != null)
                    changesList.Add(name, null);
                else if (oldValue != null && !oldValue.Equals(newValue))
                    changesList.Add(name, oldValue.ToString());
            }

            if (changesList?.Any() != true)
                return new MethodStatus<TSource>(StatusEnum.Success, entity);

            _unitOfWork.Update(entity, currentUser, changesList);
            return await SaveChangesAsync(entity, save);
        }

        public async Task<StatusEnum> SaveChangesAsync()
        {
            await _unitOfWork.SaveChangesAsync();
            return StatusEnum.Success;
        }

        public async Task<MethodStatus<TModel>> SaveChangesAsync<TModel>(TModel model, bool save) where TModel : class
        {
            if (!save)
                return new MethodStatus<TModel>(model == null ? StatusEnum.Failed : StatusEnum.Success, model);

            var saveStatus = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false) > 0;
            return new MethodStatus<TModel>(StatusEnum.Success, model);
        }
    }
}