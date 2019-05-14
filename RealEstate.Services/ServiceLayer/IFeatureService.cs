using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Services.Base;
using RealEstate.Services.Database;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using RealEstate.Services.ViewModels.Input;
using RealEstate.Services.ViewModels.Json;
using RealEstate.Services.ViewModels.ModelBind;
using RealEstate.Services.ViewModels.Search;

namespace RealEstate.Services.ServiceLayer
{
    public interface IFeatureService
    {
        Task<PaginationViewModel<CategoryViewModel>> CategoryListAsync(CategorySearchViewModel searchModel);

        Task<List<FacilityViewModel>> FacilityListAsync();

        Task<StatusEnum> FacilityRemoveAsync(string id);

        Task<MethodStatus<Feature>> FeatureUpdateAsync(FeatureInputViewModel model, bool save);

        Task<PaginationViewModel<FacilityViewModel>> FacilityListAsync(FacilitySearchViewModel searchModel);

        Task<Facility> FacilityEntityAsync(string id);

        Task<List<CategoryViewModel>> CategoryListAsync();

        Task<MethodStatus<Feature>> FeatureAddAsync(FeatureInputViewModel model, bool save);

        Task<MethodStatus<Facility>> FacilityAddAsync(FacilityInputViewModel model, bool save);

        Task<List<FeatureJsonViewModel>> FeatureListJsonAsync(FeatureTypeEnum type);

        Task<MethodStatus<Facility>> FacilityUpdateAsync(FacilityInputViewModel model, bool save);

        Task<List<FeatureViewModel>> FeatureListAsync(params FeatureTypeEnum[] type);

        Task<PaginationViewModel<FeatureViewModel>> FeatureListAsync(FeatureSearchViewModel searchModel);

        Task<MethodStatus<Feature>> FeatureAddOrUpdateAsync(FeatureInputViewModel model, bool update, bool save);

        Task<StatusEnum> FeatureRemoveAsync(string id);

        Task<Feature> FeatureEntityAsync(string id);

        Task<FeatureInputViewModel> FeatureInputAsync(string id);

        Task<CategoryInputViewModel> CategoryInputAsync(string id);

        Task<MethodStatus<Category>> CategoryAddOrUpdateAsync(CategoryInputViewModel model, bool update, bool save);

        Task<StatusEnum> CategoryRemoveAsync(string id);

        Task<MethodStatus<Facility>> FacilityAddOrUpdateAsync(FacilityInputViewModel model, bool update, bool save);

        Task<MethodStatus<Category>> CategoryAddAsync(CategoryInputViewModel model, bool save);

        Task<FacilityInputViewModel> FacilityInputAsync(string id);

        Task<MethodStatus<Category>> CategoryUpdateAsync(CategoryInputViewModel model, bool save);

        Task<Category> CategoryEntityAsync(string id);

        Task<List<CategoryViewModel>> CategoryListAsync(CategoryTypeEnum? category, bool byUserPrevilege);
    }

    public class FeatureService : IFeatureService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseService _baseService;
        private readonly DbSet<UserPropertyCategory> _userPropertyCategories;
        private readonly DbSet<UserItemCategory> _userItemCategories;
        private readonly DbSet<Category> _categories;
        private readonly DbSet<Feature> _features;
        private readonly DbSet<Facility> _facilities;

        public FeatureService(
            IUnitOfWork unitOfWork,
            IBaseService baseService
            )
        {
            _unitOfWork = unitOfWork;
            _baseService = baseService;
            _userPropertyCategories = _unitOfWork.Set<UserPropertyCategory>();
            _userItemCategories = _unitOfWork.Set<UserItemCategory>();
            _categories = _unitOfWork.Set<Category>();
            _features = _unitOfWork.Set<Feature>();
            _facilities = _unitOfWork.Set<Facility>();
        }

        public async Task<FeatureInputViewModel> FeatureInputAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return default;

            var query = _features.Where(x => x.Id == id)
                .Include(x => x.ApplicantFeatures)
                .Include(x => x.ItemFeatures)
                .Include(x => x.PropertyFeatures);

            var entity = await query.FirstOrDefaultAsync().ConfigureAwait(false);
            var viewModel = entity.Into<Feature, FeatureViewModel>();
            if (viewModel == null)
                return default;

            var result = new FeatureInputViewModel
            {
                Id = viewModel.Id,
                Type = viewModel.Type,
                Name = viewModel.Name
            };

            return result;
        }

        public async Task<FacilityInputViewModel> FacilityInputAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return default;

            var query = _facilities.Where(x => x.Id == id)
                .Include(x => x.PropertyFacilities);

            var entity = await query.FirstOrDefaultAsync().ConfigureAwait(false);
            var viewModel = entity.Into<Facility, FacilityViewModel>();
            if (viewModel == null)
                return default;

            var result = new FacilityInputViewModel
            {
                Id = viewModel.Id,
                Name = viewModel.Name
            };

            return result;
        }

        public async Task<CategoryInputViewModel> CategoryInputAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return default;

            var query = _categories.Where(x => x.Id == id);
            var entity = await query.FirstOrDefaultAsync().ConfigureAwait(false);
            var viewModel = entity.Into<Category, CategoryViewModel>();
            if (viewModel == null)
                return default;

            var result = new CategoryInputViewModel
            {
                Id = viewModel.Id,
                Type = viewModel.Type,
                Name = viewModel.Name
            };

            return result;
        }

        public async Task<Facility> FacilityEntityAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return default;

            var result = await _facilities.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            return result;
        }

        public async Task<Feature> FeatureEntityAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return default;

            var result = await _features.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            return result;
        }

        public async Task<Category> CategoryEntityAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return default;

            var result = await _categories.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            return result;
        }

        public async Task<StatusEnum> FacilityRemoveAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return StatusEnum.ParamIsNull;

            var entity = await _facilities.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            var result = await _baseService.RemoveAsync(entity,
                    new[]
                    {
                        Role.SuperAdmin, Role.Admin
                    },
                    true,
                    true)
                .ConfigureAwait(false);

            return result;
        }

        public async Task<StatusEnum> FeatureRemoveAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return StatusEnum.ParamIsNull;

            var entity = await _features.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            var result = await _baseService.RemoveAsync(entity,
                    new[]
                    {
                        Role.SuperAdmin, Role.Admin
                    },
                    true,
                    true)
                .ConfigureAwait(false);

            return result;
        }

        public async Task<StatusEnum> CategoryRemoveAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return StatusEnum.ParamIsNull;

            var entity = await _categories.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            var result = await _baseService.RemoveAsync(entity,
                    new[]
                    {
                        Role.SuperAdmin, Role.Admin
                    },
                    true,
                    true)
                .ConfigureAwait(false);

            return result;
        }

        public async Task<MethodStatus<Feature>> FeatureUpdateAsync(FeatureInputViewModel model, bool save)
        {
            if (model == null)
                return new MethodStatus<Feature>(StatusEnum.ModelIsNull, null);

            if (model.IsNew)
                return new MethodStatus<Feature>(StatusEnum.IdIsNull, null);

            var entity = await FeatureEntityAsync(model.Id).ConfigureAwait(false);
            var updateStatus = await _baseService.UpdateAsync(entity,
                _ =>
                {
                    entity.Type = model.Type;
                    entity.Name = model.Name;
                }, new[]
                {
                    Role.SuperAdmin
                }, save, StatusEnum.UserIsNull).ConfigureAwait(false);
            return updateStatus;
        }

        public async Task<MethodStatus<Facility>> FacilityUpdateAsync(FacilityInputViewModel model, bool save)
        {
            if (model == null)
                return new MethodStatus<Facility>(StatusEnum.ModelIsNull, null);

            if (model.IsNew)
                return new MethodStatus<Facility>(StatusEnum.IdIsNull, null);

            var entity = await FacilityEntityAsync(model.Id).ConfigureAwait(false);
            var updateStatus = await _baseService.UpdateAsync(entity,
                _ => entity.Name = model.Name,
                new[]
                {
                    Role.SuperAdmin
                }, save, StatusEnum.UserIsNull).ConfigureAwait(false);
            return updateStatus;
        }

        public async Task<MethodStatus<Category>> CategoryUpdateAsync(CategoryInputViewModel model, bool save)
        {
            if (model == null)
                return new MethodStatus<Category>(StatusEnum.ModelIsNull, null);

            if (model.IsNew)
                return new MethodStatus<Category>(StatusEnum.IdIsNull, null);

            var entity = await CategoryEntityAsync(model.Id).ConfigureAwait(false);
            var updateStatus = await _baseService.UpdateAsync(entity,
                _ =>
                {
                    entity.Type = model.Type;
                    entity.Name = model.Name;
                }, new[]
                {
                    Role.SuperAdmin
                }, save, StatusEnum.UserIsNull).ConfigureAwait(false);
            return updateStatus;
        }

        public Task<MethodStatus<Facility>> FacilityAddOrUpdateAsync(FacilityInputViewModel model, bool update, bool save)
        {
            return update
                ? FacilityUpdateAsync(model, save)
                : FacilityAddAsync(model, save);
        }

        public Task<MethodStatus<Feature>> FeatureAddOrUpdateAsync(FeatureInputViewModel model, bool update, bool save)
        {
            return update
                ? FeatureUpdateAsync(model, save)
                : FeatureAddAsync(model, save);
        }

        public Task<MethodStatus<Category>> CategoryAddOrUpdateAsync(CategoryInputViewModel model, bool update, bool save)
        {
            return update
                ? CategoryUpdateAsync(model, save)
                : CategoryAddAsync(model, save);
        }

        public async Task<MethodStatus<Facility>> FacilityAddAsync(FacilityInputViewModel model, bool save)
        {
            if (model == null)
                return new MethodStatus<Facility>(StatusEnum.ModelIsNull, null);

            var addStatus = await _baseService.AddAsync(new Facility
            {
                Name = model.Name,
            }, new[]
            {
                Role.SuperAdmin, Role.Admin
            }, save).ConfigureAwait(false);
            return addStatus;
        }

        public async Task<MethodStatus<Feature>> FeatureAddAsync(FeatureInputViewModel model, bool save)
        {
            if (model == null)
                return new MethodStatus<Feature>(StatusEnum.ModelIsNull, null);

            var addStatus = await _baseService.AddAsync(new Feature
            {
                Name = model.Name,
                Type = model.Type,
            }, new[]
            {
                Role.SuperAdmin, Role.Admin
            }, save).ConfigureAwait(false);
            return addStatus;
        }

        public async Task<MethodStatus<Category>> CategoryAddAsync(CategoryInputViewModel model, bool save)
        {
            if (model == null)
                return new MethodStatus<Category>(StatusEnum.ModelIsNull, null);

            var addStatus = await _baseService.AddAsync(new Category
            {
                Name = model.Name,
                Type = model.Type,
            }, new[]
            {
                Role.SuperAdmin, Role.Admin
            }, save).ConfigureAwait(false);
            return addStatus;
        }

        public async Task<PaginationViewModel<FeatureViewModel>> FeatureListAsync(FeatureSearchViewModel searchModel)
        {
            var query = _baseService.CheckDeletedItemsPrevillege(_features, searchModel, out var currentUser);
            if (query == null)
                return new PaginationViewModel<FeatureViewModel>();

            if (searchModel != null)
            {
                if (!string.IsNullOrEmpty(searchModel.Name))
                    query = query.Where(x => EF.Functions.Like(x.Name, searchModel.Name.Like()));

                if (searchModel.Type != null)
                    query = query.Where(x => x.Type == searchModel.Type);

                query = _baseService.AdminSeachConditions(query, searchModel);
            }

            var result = await _baseService.PaginateAsync(query, searchModel?.PageNo ?? 1,
                item => item.Into<Feature, FeatureViewModel>()
            ).ConfigureAwait(false);

            return result;
        }

        public async Task<PaginationViewModel<FacilityViewModel>> FacilityListAsync(FacilitySearchViewModel searchModel)
        {
            var query = _baseService.CheckDeletedItemsPrevillege(_facilities, searchModel, out var currentUser);
            if (query == null)
                return new PaginationViewModel<FacilityViewModel>();

            if (searchModel != null)
            {
                if (!string.IsNullOrEmpty(searchModel.Name))
                    query = query.Where(x => EF.Functions.Like(x.Name, searchModel.Name.Like()));

                query = _baseService.AdminSeachConditions(query, searchModel);
            }
            var result = await _baseService.PaginateAsync(query, searchModel?.PageNo ?? 1,
                item => item.Into<Facility, FacilityViewModel>()
            ).ConfigureAwait(false);

            return result;
        }

        public async Task<List<FeatureJsonViewModel>> FeatureListJsonAsync(FeatureTypeEnum type)
        {
            var models = await (from feature in _features
                                where feature.Type == type
                                select feature).ToListAsync().ConfigureAwait(false);
            if (models?.Any() != true)
                return default;

            var result = models.Select(x => new FeatureJsonViewModel
            {
                Name = x.Name,
                Id = x.Id
            }).ToList();

            return result;
        }

        public async Task<List<FacilityViewModel>> FacilityListAsync()
        {
            var query = _facilities as IQueryable<Facility>;
            query = query.WhereNotDeleted();

            var facilities = await query.ToListAsync().ConfigureAwait(false);
            return facilities.Into<Facility, FacilityViewModel>();
        }

        public async Task<List<FeatureViewModel>> FeatureListAsync(params FeatureTypeEnum[] types)
        {
            var query = _features as IQueryable<Feature>;
            query = query.WhereNotDeleted();

            if (types?.Any() == true)
                query = query.Where(x => types.Contains(x.Type));

            var features = await query.ToListAsync().ConfigureAwait(false);
            return features.Into<Feature, FeatureViewModel>();
        }

        public async Task<PaginationViewModel<CategoryViewModel>> CategoryListAsync(CategorySearchViewModel searchModel)
        {
            var query = _baseService.CheckDeletedItemsPrevillege(_categories, searchModel, out var currentUser);
            if (query == null)
                return new PaginationViewModel<CategoryViewModel>();

            if (searchModel != null)
            {
                if (!string.IsNullOrEmpty(searchModel.Name))
                    query = query.Where(x => EF.Functions.Like(x.Name, searchModel.Name.Like()));

                if (!string.IsNullOrEmpty(searchModel.Id))
                    query = query.Where(x => EF.Functions.Like(x.Name, searchModel.Id.Like()));

                if (searchModel.Type != null)
                    query = query.Where(x => x.Type == searchModel.Type);

                query = _baseService.AdminSeachConditions(query, searchModel);
            }
            var result = await _baseService.PaginateAsync(query, searchModel?.PageNo ?? 1,
                item => item.Into<Category, CategoryViewModel>()
            ).ConfigureAwait(false);

            return result;
        }

        public async Task<List<CategoryViewModel>> CategoryListAsync()
        {
            var query = _categories.IgnoreQueryFilters();
            var categories = await query.ToListAsync().ConfigureAwait(false);
            return categories.Into<Category, CategoryViewModel>();
        }

        public async Task<List<CategoryViewModel>> CategoryListAsync(CategoryTypeEnum? category, bool byUserPrevilege)
        {
            var query = _categories.AsQueryable();
            query = query.WhereNotDeleted();

            if (category != null)
            {
                query = query.Where(x => x.Type == category);
                if (byUserPrevilege)
                {
                    var user = _baseService.CurrentUser();
                    if (user == null)
                        return default;

                    switch (category)
                    {
                        case CategoryTypeEnum.Property:
                            query = query.Where(x => user.UserPropertyCategories.Any(c => c.CategoryId == x.Id));
                            break;

                        case CategoryTypeEnum.Item:
                        default:
                            query = query.Where(x => user.UserItemCategories.Any(c => c.CategoryId == x.Id));
                            break;
                    }
                }
            }

            var categories = await query.ToListAsync().ConfigureAwait(false);
            return categories.Into<Category, CategoryViewModel>();
        }
    }
}