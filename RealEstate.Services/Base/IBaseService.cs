using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Domain;
using RealEstate.Domain.Base;
using RealEstate.Domain.Tables;
using RealEstate.Extensions;
using RealEstate.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RealEstate.Services.Base
{
    public interface IBaseService
    {
        UserViewModel CurrentUser();

        TModel Map<TSource, TModel>(TSource query,
            TModel entity) where TSource : class where TModel : class;

        Task<(StatusEnum, TSource)> AddAsync<TSource>(Func<UserViewModel, TSource> entity,
            Role[] allowedRoles, bool save) where TSource : BaseEntity;

        Task<(StatusEnum, TSource)> AddAsync<TSource>(DbSet<TSource> entities, Expression<Func<TSource, bool>> duplicateCondition, TSource entity,
            Role[] allowedRoles, bool save) where TSource : BaseEntity;

        Task<StatusEnum> RemoveAsync<TEntity>(TEntity entity, Role[] allowedRoles, bool undeleteAllowed, bool save)
            where TEntity : BaseEntity;

        Task<(StatusEnum, TSource)> AddAsync<TSource>(TSource entity,
            Role[] allowedRoles, bool save) where TSource : BaseEntity;

        bool IsAllowed(Role[] roles);

        Task<(StatusEnum, TSource)> UpdateAsync<TSource>(TSource entity,
            Action changes, Role[] allowedRoles, bool save, StatusEnum modelNullStatus) where TSource : BaseEntity;

        Task<PaginationViewModel<TOutput>> PaginateAsync<TQuery, TOutput>(IQueryable<TQuery> query, int page, Func<TQuery, TOutput> viewModel,
            Role[] allowedRolesToIncludeDeleted) where TQuery : BaseEntity where TOutput : class;

        (TModel, List<LogUserViewModel>) SelectAndTrack<TSource, TModel>(TSource model, Func<TSource, TModel> expression,
            List<LogUserViewModel> users) where TSource : class where TModel : class;

        Task<StatusEnum> SaveChangesAsync(bool save);

        List<TOutput> Map<TSource, TOutput>(List<TSource> models, Func<TSource, TOutput> map)
            where TSource : class where TOutput : class;

        List<TModel> SelectAndTrack<TSource, TModel>(List<TSource> model,
            Func<TSource, TModel> expression) where TSource : class where TModel : class;

        Task<StatusEnum> RemoveAsync<TEntity>(TEntity entity, bool undeleteAllowed, bool save)
            where TEntity : BaseEntity;

        Task<StatusEnum> SyncAsync<TSource, TModel>(ICollection<TSource> entities,
            List<TModel> syncWith, Func<TModel, UserViewModel, TSource> newEntity, Expression<Func<TSource, TModel, bool>> indentifier, Role[] allowedRoles, bool save)
            where TSource : BaseEntity;

        Task<(StatusEnum, TModel)> SaveChangesAsync<TModel>(TModel model, bool save) where TModel : class;

        UserViewModel CurrentUser(List<Claim> claims);
    }

    public class BaseService : IBaseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _accessor;
        private readonly DbSet<User> _users;
        private readonly DbSet<Log> _logs;

        public BaseService(
            IUnitOfWork unitOfWork,
            IHttpContextAccessor accessor
            )
        {
            _unitOfWork = unitOfWork;
            _accessor = accessor;
            _users = _unitOfWork.Set<User>();
            _logs = _unitOfWork.Set<Log>();
        }

        public async Task<PaginationViewModel<TOutput>> PaginateAsync<TQuery, TOutput>(IQueryable<TQuery> query, int page, Func<TQuery, TOutput> viewModel,
            Role[] allowedRolesToIncludeDeleted) where TQuery : BaseEntity where TOutput : class
        {
            page = page <= 1 ? 1 : page;
            const int pageSize = 10;

            var currentUser = CurrentUser();
            if (currentUser == null)
                return default;

            if (IsAllowed(allowedRolesToIncludeDeleted))
                query = query.IgnoreQueryFilters();

            query = query.OrderByDescending(x => x.DateTime);

            var pagingQuery = page > 1
                ? query.Skip(pageSize * (page - 1))
                : query;

            var entities = await pagingQuery.Take(pageSize).ToListAsync()
                .ConfigureAwait(false);
            var count = await query.CountAsync().ConfigureAwait(false);

            if (entities == null) return default;
            var viewList = SelectAndTrack(entities, viewModel);

            var output = new PaginationViewModel<TOutput>();
            var outputType = output.GetType();

            var pagesProperty = outputType.GetProperty(nameof(output.Pages));
            var pageProperty = outputType.GetProperty(nameof(output.CurrentPage));
            var itemsProperty = outputType.GetProperty(nameof(output.Items));

            pagesProperty.SetValue(output, NumberProcessorExtension.RoundToUp((double)count / pageSize));
            pageProperty.SetValue(output, page);
            itemsProperty.SetValue(output, viewList);

            return output;
        }

        private HttpContext HttpContext => _accessor.HttpContext;

        public UserViewModel CurrentUser()
        {
            var context = HttpContext;
            var users = context?.User;
            if (users == null) return null;

            var claims = users.Claims.ToList();
            return CurrentUser(claims);
        }

        public async Task<(StatusEnum, TSource)> AddAsync<TSource>(DbSet<TSource> entities, Expression<Func<TSource, bool>> duplicateCondition, TSource entity,
            Role[] allowedRoles, bool save) where TSource : BaseEntity
        {
            var duplicate = await entities.FirstOrDefaultAsync(duplicateCondition).ConfigureAwait(false);
            if (duplicate != null)
                return new ValueTuple<StatusEnum, TSource>(StatusEnum.AlreadyExists, null);

            return await AddAsync(entity, allowedRoles, save).ConfigureAwait(false);
        }

        public async Task<(StatusEnum, TSource)> AddAsync<TSource>(Func<UserViewModel, TSource> entity,
            Role[] allowedRoles, bool save) where TSource : BaseEntity
        {
            var currentUser = CurrentUser();
            if (currentUser == null)
                return new ValueTuple<StatusEnum, TSource>(StatusEnum.UserIsNull, null);

            if (!IsAllowed(allowedRoles))
                return new ValueTuple<StatusEnum, TSource>(StatusEnum.ForbiddenAndUnableToUpdateOrShow, null);

            _unitOfWork.Add(entity, currentUser.Id);
            return await SaveChangesAsync(entity.Invoke(currentUser), save).ConfigureAwait(false);
        }

        public async Task<(StatusEnum, TSource)> AddAsync<TSource>(TSource entity,
            Role[] allowedRoles, bool save) where TSource : BaseEntity
        {
            var currentUser = CurrentUser();
            if (currentUser == null)
                return new ValueTuple<StatusEnum, TSource>(StatusEnum.UserIsNull, null);

            if (!IsAllowed(allowedRoles))
                return new ValueTuple<StatusEnum, TSource>(StatusEnum.ForbiddenAndUnableToUpdateOrShow, null);

            _unitOfWork.Add(entity, currentUser.Id);
            return await SaveChangesAsync(entity, save).ConfigureAwait(false);
        }

        public List<TOutput> Map<TSource, TOutput>(List<TSource> models, Func<TSource, TOutput> map)
            where TSource : class where TOutput : class
        {
            var isTracked = models.GetType().HasBaseType(typeof(BaseEntity));
            var result = isTracked
                ? SelectAndTrack(models, map)
                : models.Select(map).ToList();
            return result;
        }

        public List<TModel> SelectAndTrack<TSource, TModel>(List<TSource> model,
            Func<TSource, TModel> expression) where TSource : class where TModel : class
        {
            var users = new List<LogUserViewModel>();
            var finalList = new List<TModel>();
            if (model?.Any() != true) return default;

            foreach (var source in model)
            {
                var (trackedEntity, usersUpdated) = SelectAndTrack(source, expression, users);
                users = usersUpdated;
                finalList.Add(trackedEntity);
            }

            return finalList;
        }

        public (TModel, List<LogUserViewModel>) SelectAndTrack<TSource, TModel>(TSource model, Func<TSource, TModel> expression,
                   List<LogUserViewModel> users) where TSource : class where TModel : class
        {
            if (model == null) return default;
            var viewModel = expression.Invoke(model);

            var trackProperty = model.GetType().GetProperty("Tracks");
            if (trackProperty == null || !(trackProperty.GetValue(model) is ICollection<Log> entityTracks))
                return new ValueTuple<TModel, List<LogUserViewModel>>(viewModel, users);

            var templateViewModel = new BaseLogViewModel();
            var entityTracksInfo = viewModel.GetType().GetProperty(nameof(templateViewModel.Logs));
            if (entityTracksInfo == null || entityTracksInfo.PropertyType != typeof(List<LogViewModel>))
                return new ValueTuple<TModel, List<LogUserViewModel>>(viewModel, users);

            var viewTracks = new List<LogViewModel>();
            if (entityTracks.Count == 0)
                return new ValueTuple<TModel, List<LogUserViewModel>>(viewModel, users);

            if (users == null)
                users = new List<LogUserViewModel>();

            var lastTracks = new List<Log>
            {
                entityTracks.OrderByDescending(x => x.DateTime).FirstOrDefault(x => x.Type == LogTypeEnum.Create),
                entityTracks.OrderByDescending(x => x.DateTime).FirstOrDefault(x => x.Type == LogTypeEnum.Modify || x.Type == LogTypeEnum.Undelete),
                entityTracks.OrderByDescending(x => x.DateTime).FirstOrDefault(x => x.Type == LogTypeEnum.Delete)
            }.Where(x => x != null).ToList();

            var packedTracks = from trc in lastTracks
                               join creator in _users on trc.CreatorId equals creator.Id
                               select new
                               {
                                   Track = trc,
                                   Creator = creator
                               };

            foreach (var packedTrack in packedTracks)
            {
                var thisUser = packedTrack.Creator;
                var thisTrack = packedTrack.Track;

                var alreadyAddedUser = users.Find(x => x.Id == packedTrack.Creator.Id);
                if (alreadyAddedUser == null)
                {
                    users.Add(new LogUserViewModel
                    {
                        FirstName = thisUser.FirstName,
                        LastName = thisUser.LastName,
                        Mobile = thisUser.Mobile,
                        Role = thisUser.Role,
                        Username = thisUser.Username,
                        Id = thisUser.Id
                    });
                }

                viewTracks.Add(new LogViewModel
                {
                    Type = thisTrack.Type,
                    DateTime = thisTrack.DateTime,
                    Id = thisTrack.Id,
                    User = users.Find(x => x.Id == thisUser.Id)
                });
            }

            entityTracksInfo.SetValue(viewModel, viewTracks);
            return new ValueTuple<TModel, List<LogUserViewModel>>(viewModel, users);
        }

        public async Task<StatusEnum> RemoveAsync<TEntity>(TEntity entity, bool undeleteAllowed, bool save)
            where TEntity : BaseEntity
        {
            var currentUser = CurrentUser();
            if (currentUser == null)
                return StatusEnum.UserIsNull;

            var lastLog = entity.LastLog();
            if (lastLog?.Type == LogTypeEnum.Delete)
            {
                if (undeleteAllowed)
                    _unitOfWork.UnDelete(entity, currentUser.Id);
                else
                    return StatusEnum.AlreadyDeleted;
            }
            else
            {
                _unitOfWork.Delete(entity, currentUser.Id);
            }

            return await SaveChangesAsync(save).ConfigureAwait(false);
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

        public async Task<StatusEnum> SyncAsync<TSource, TModel>(ICollection<TSource> entities,
            List<TModel> syncWith, Func<TModel, UserViewModel, TSource> newEntity, Expression<Func<TSource, TModel, bool>> indentifier, Role[] allowedRoles, bool save) where TSource : BaseEntity
        {
            var currentUser = CurrentUser();
            if (currentUser == null) return StatusEnum.UserIsNull;

            var mustBePresent = entities.Where(entity => syncWith.Any(model => indentifier.Compile().Invoke(entity, model))).ToList();
            var mustBeRemoved = entities.Where(x => !mustBePresent.Contains(x)).ToList();
            if (mustBeRemoved.Count > 0)
                foreach (var redundant in mustBeRemoved)
                    await RemoveAsync(redundant, false, false).ConfigureAwait(false);

            if (syncWith?.Any() == true)
            {
                foreach (var model in syncWith)
                {
                    var _model = mustBePresent.Find(entity => indentifier.Compile().Invoke(entity, model));
                    if (_model == null)
                    {
                        var newItem = newEntity.Invoke(model, currentUser);
                        await AddAsync(newItem, allowedRoles, false).ConfigureAwait(false);
                    }
                    else
                    {
                        var lastLog = _model.LastLog();
                        if (lastLog?.Type != LogTypeEnum.Delete)
                            continue;

                        _unitOfWork.UnDelete(_model, currentUser.Id);
                    }
                }
            }

            return await SaveChangesAsync(save).ConfigureAwait(false);
        }

        public UserViewModel CurrentUser(List<Claim> claims)
        {
            if (claims.Count == 0) return null;
            var result = new UserViewModel
            {
                Username = claims.Find(x => x.Type == ClaimTypes.Name)?.Value,
                Mobile = claims.Find(x => x.Type == ClaimTypes.MobilePhone)?.Value,
                Role = claims.Find(x => x.Type == ClaimTypes.Role)?.Value.To<Role>() ?? Role.User,
                EncryptedPassword = claims.Find(x => x.Type == ClaimTypes.Hash)?.Value,
                Id = claims.Find(x => x.Type == ClaimTypes.NameIdentifier)?.Value,
                FirstName = claims.Find(x => x.Type == "FirstName")?.Value,
                LastName = claims.Find(x => x.Type == "LastName")?.Value,
                Address = claims.Find(x => x.Type == ClaimTypes.StreetAddress)?.Value,
                Phone = claims.Find(x => x.Type == ClaimTypes.HomePhone)?.Value,
                CreationDateTime = DateTime.Parse(claims.Find(x => x.Type == "CreationDateTime")?.Value),
                PropertyCategories = claims.Find(x => x.Type == "ItemCategories")?.Value.JsonConversion<List<CategoryViewModel>>(),
                ItemCategories = claims.Find(x => x.Type == "PropertyCategories")?.Value.JsonConversion<List<CategoryViewModel>>(),
            };
            return result;
        }

        public async Task<StatusEnum> RemoveAsync<TEntity>(TEntity entity, Role[] allowedRoles, bool undeleteAllowed, bool save)
            where TEntity : BaseEntity
        {
            if (!IsAllowed(allowedRoles))
                return StatusEnum.Forbidden;

            if (entity == null)
                return StatusEnum.ModelIsNull;

            return await RemoveAsync(entity, undeleteAllowed, save).ConfigureAwait(false);
        }

        public bool IsAllowed(Role[] roles)
        {
            var currentUser = CurrentUser();
            if (currentUser == null)
                return false;

            return roles?.Any() != true || roles.Any(x => x == currentUser.Role);
        }

        public async Task<(StatusEnum, TSource)> UpdateAsync<TSource>(TSource entity,
         Action changes, Role[] allowedRoles, bool save, StatusEnum modelNullStatus) where TSource : BaseEntity
        {
            var currentUser = CurrentUser();
            if (currentUser == null)
                return new ValueTuple<StatusEnum, TSource>(StatusEnum.UserIsNull, null);

            if (!IsAllowed(allowedRoles))
                return new ValueTuple<StatusEnum, TSource>(StatusEnum.Forbidden, null);

            if (entity == null)
                return new ValueTuple<StatusEnum, TSource>(modelNullStatus, null);

            changes.Invoke();
            _unitOfWork.Update(entity, currentUser.Id);

            return await SaveChangesAsync(entity, save).ConfigureAwait(false);
        }

        public async Task<StatusEnum> SaveChangesAsync(bool save)
        {
            if (!save)
                return StatusEnum.BasedOnModelNullability;

            var saveStatus = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false) > 0;
            return saveStatus
                ? StatusEnum.Success
                : StatusEnum.UnableToSave;
        }

        public async Task<(StatusEnum, TModel)> SaveChangesAsync<TModel>(TModel model, bool save) where TModel : class
        {
            if (!save)
                return new ValueTuple<StatusEnum, TModel>(StatusEnum.BasedOnModelNullability, model);

            var saveStatus = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false) > 0;
            return saveStatus
                ? new ValueTuple<StatusEnum, TModel>(StatusEnum.Success, model)
                : new ValueTuple<StatusEnum, TModel>(StatusEnum.UnableToSave, null);
        }
    }
}