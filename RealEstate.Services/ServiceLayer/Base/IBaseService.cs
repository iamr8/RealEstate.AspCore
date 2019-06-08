using EFSecondLevelCache.Core;
using EFSecondLevelCache.Core.Contracts;
using GeoAPI.Geometries;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using MoreLinq;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database;
using RealEstate.Services.Database.Base;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using RealEstate.Services.ViewModels;
using RealEstate.Services.ViewModels.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
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

        IQueryable<TSource> CheckDeletedItemsPrevillege<TSource, TSearch>(IQueryable<TSource> source, TSearch searchModel, out CurrentUserViewModel currentUser)
            where TSource : BaseEntity where TSearch : BaseSearchModel;

        IQueryable<TSource> CheckDeletedItemsPrevillege<TSource, TSearch>(DbSet<TSource> source, TSearch searchModel, out CurrentUserViewModel currentUser)
            where TSource : BaseEntity where TSearch : BaseSearchModel;

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
            CurrentUserViewModel currentUser = null) where TQuery : BaseEntity where TOutput : BaseLogViewModel where TSearch : BaseSearchModel;

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
        private readonly IFileHandler _fileHandler;
        private readonly DbSet<User> _users;

        public BaseService(
            IUnitOfWork unitOfWork,
            IFileHandler fileHandler,
            IHttpContextAccessor accessor
            )
        {
            _unitOfWork = unitOfWork;
            _fileHandler = fileHandler;
            _accessor = accessor;
            _users = _unitOfWork.Set<User>();
        }

        public async Task<PaginationViewModel<TOutput>> PaginateAsync<TQuery, TOutput, TSearch>(IQueryable<TQuery> query, TSearch searchModel, Func<TQuery, TOutput> viewModel, CurrentUserViewModel currentUser = null)
            where TQuery : BaseEntity where TOutput : BaseLogViewModel where TSearch : BaseSearchModel
        {
            var output = new PaginationViewModel<TOutput>();

            currentUser = currentUser ?? CurrentUser();
            if (currentUser == null)
                return output;

            var page = searchModel?.PageNo ?? 1;
            page = page <= 1 ? 1 : page;
            const int pageSize = 10;

            var cacheKeyBuilder = new StringBuilder();
            searchModel?.GetType().GetPublicProperties().ToList()?.ForEach(searchProperty =>
            {
                cacheKeyBuilder.AppendKey(searchProperty.Name, searchProperty.GetValue(searchModel));
            });
            var cacheKey = cacheKeyBuilder.ToString();

            query = query.OrderDescendingByCreationDateTime();
            var pagingQuery = page > 1
                ? query.Skip(pageSize * (page - 1))
                : query;
            pagingQuery = pagingQuery.Take(pageSize);

            var efPolicy = string.IsNullOrEmpty(cacheKey)
                ? new EFCachePolicy(CacheExpirationMode.Sliding, TimeSpan.FromDays(1))
                : new EFCachePolicy(CacheExpirationMode.Sliding, TimeSpan.FromDays(1), cacheKey);
            var efDebug = new EFCacheDebugInfo();

            var entities = await pagingQuery
                .Cacheable(efPolicy, efDebug)
                .ToListAsync()
                .ConfigureAwait(false);
            if (entities?.Any() != true)
                return output;

            var rowCount = await query.Cacheable().CountAsync().ConfigureAwait(false);

            var viewList = entities
                .Select(viewModel.Invoke)
                .ToHasNotNullList();

            if (viewList == null)
                return output;

            output.CurrentPage = page;
            output.Pages = NumberProcessorExtensions.RoundToUp((double)rowCount / pageSize);
            output.Items = viewList;

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

            var query = source.AsQueryable();
            if (searchModel?.IncludeDeletedItems == true && (currentUser.Role == Role.Admin || currentUser.Role == Role.SuperAdmin))
                query = query.IgnoreQueryFilters();

            return query;
        }

        public IQueryable<TSource> CheckDeletedItemsPrevillege<TSource, TSearch>(IQueryable<TSource> source, TSearch searchModel, out CurrentUserViewModel currentUser) where TSource : BaseEntity where TSearch : BaseSearchModel
        {
            currentUser = CurrentUser();
            if (currentUser == null)
                return null;

            var query = source.AsQueryable();
            if (searchModel?.IncludeDeletedItems == true && (currentUser.Role == Role.Admin || currentUser.Role == Role.SuperAdmin))
                query = query.IgnoreQueryFilters();

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
            var duplicate = await entities.FirstOrDefaultAsync(duplicateCondition).ConfigureAwait(false);
            if (duplicate != null)
                return new MethodStatus<TSource>(StatusEnum.AlreadyExists, null);

            return await AddAsync(entity, allowedRoles, save).ConfigureAwait(false);
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
            return await SaveChangesAsync(finalEntity, save).ConfigureAwait(false);
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
            return await SaveChangesAsync(entity, save).ConfigureAwait(false);
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
                _unitOfWork.Delete(entity, currentUser);
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

            return await SaveChangesAsync().ConfigureAwait(false);
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
                        await RemoveAsync(redundant, null, DeleteEnum.Delete, false).ConfigureAwait(false);
            }

            if (newList?.Any() != true)
                return await SaveChangesAsync().ConfigureAwait(false);

            foreach (var model in newList)
            {
                var source = currentListEntities?.FirstOrDefault(ent => indentifier.Compile().Invoke(ent, model));
                if (source == null)
                {
                    var newItem = newEntity.Invoke(model);
                    await AddAsync(newItem, allowedRoles, false).ConfigureAwait(false);
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

            return await SaveChangesAsync().ConfigureAwait(false);
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
                        await RemoveAsync(redundant, null, DeleteEnum.Hide, false).ConfigureAwait(false);
            }

            if (newList?.Any() != true)
                return await SaveChangesAsync().ConfigureAwait(false);

            foreach (var model in newList)
            {
                var source = currentListEntities?.FirstOrDefault(entity => indentifier.Compile().Invoke(entity, model));
                if (source == null)
                {
                    var newItem = newEntity.Invoke(model, currentUser);
                    await AddAsync(newItem, allowedRoles, false).ConfigureAwait(false);
                }
                else
                {
                    if (!source.IsDeleted)
                        continue;

                    _unitOfWork.UnDelete(source, currentUser);
                }
            }

            return await SaveChangesAsync().ConfigureAwait(false);
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
            return await SaveChangesAsync(entity, save).ConfigureAwait(false);
        }

        public async Task<StatusEnum> SaveChangesAsync()
        {
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
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