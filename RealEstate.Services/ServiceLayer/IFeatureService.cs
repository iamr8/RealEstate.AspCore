using EFSecondLevelCache.Core;
using Microsoft.EntityFrameworkCore;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Services.Database;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using RealEstate.Services.ServiceLayer.Base;
using RealEstate.Services.ViewModels.Input;
using RealEstate.Services.ViewModels.Json;
using RealEstate.Services.ViewModels.ModelBind;
using RealEstate.Services.ViewModels.Search;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public const string PricePerMeter = "01cb6a1d-959d-4abb-8488-f10ab09bd8a8";
        public const string LoanPrice = "736ad605-78ea-41e1-bdeb-8d2811db2dec";
        public const string BuildYear = "cdb97926-b3b1-48ec-bdd6-389a0431007c";
        public const string FinalPrice = "54a0b920-c17f-4ff2-9c51-f9551159026a";
        public const string ComplexUnits = "e75e0fb5-bdcd-470e-9e24-89a022d14490";
        public const string Tenant = "03fb13b9-9a42-4e0f-a77c-08ed1a3bb179";
        public const string GroundWidth = "93179c86-a0db-40cf-a0e2-d26c70a8de45";
        public const string BedRooms = "b35f4bef-925e-415b-b8f1-19f6df02e6ac";
        public const string Meterage = "15bf9d15-07bc-4f3c-8339-8192c8fd0c18";
        public const string DepositPrice = "22f68cda-29f2-4cc0-bb0f-e578defb85d1";
        public const string RentPrice = "02cbebcc-610a-4bd2-8e27-e2d50b13587f";

        public async Task<FeatureInputViewModel> FeatureInputAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return default;

            var query = _features.Where(x => x.Id == id)
                .Include(x => x.ApplicantFeatures)
                .Include(x => x.ItemFeatures)
                .Include(x => x.PropertyFeatures);

            var entity = await query.FirstOrDefaultAsync();
            var viewModel = entity.Map<FeatureViewModel>();
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

            var entity = await query.FirstOrDefaultAsync();
            var viewModel = entity.Map<FacilityViewModel>();
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
            var entity = await query.FirstOrDefaultAsync();
            var viewModel = entity.Map<CategoryViewModel>();
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

            var result = await _facilities.FirstOrDefaultAsync(x => x.Id == id);
            return result;
        }

        public async Task<Feature> FeatureEntityAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return default;

            var result = await _features.FirstOrDefaultAsync(x => x.Id == id);
            return result;
        }

        public async Task<Category> CategoryEntityAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return default;

            var result = await _categories.FirstOrDefaultAsync(x => x.Id == id);
            return result;
        }

        public async Task<StatusEnum> FacilityRemoveAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return StatusEnum.ParamIsNull;

            var entity = await _facilities.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id);
            var result = await _baseService.RemoveAsync(entity,
                    new[]
                    {
                        Role.SuperAdmin, Role.Admin
                    })
                ;

            return result;
        }

        public async Task<StatusEnum> FeatureRemoveAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return StatusEnum.ParamIsNull;

            var entity = await _features.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id);
            var result = await _baseService.RemoveAsync(entity,
                    new[]
                    {
                        Role.SuperAdmin, Role.Admin
                    })
                ;

            return result;
        }

        public async Task<StatusEnum> CategoryRemoveAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return StatusEnum.ParamIsNull;

            var entity = await _categories.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id);
            var result = await _baseService.RemoveAsync(entity,
                    new[]
                    {
                        Role.SuperAdmin, Role.Admin
                    })
                ;

            return result;
        }

        public async Task<MethodStatus<Feature>> FeatureUpdateAsync(FeatureInputViewModel model, bool save)
        {
            if (model == null)
                return new MethodStatus<Feature>(StatusEnum.ModelIsNull, null);

            if (model.IsNew)
                return new MethodStatus<Feature>(StatusEnum.IdIsNull, null);

            var entity = await FeatureEntityAsync(model.Id);
            var updateStatus = await _baseService.UpdateAsync(entity,
                _ =>
                {
                    entity.Type = model.Type;
                    entity.Name = model.Name;
                }, new[]
                {
                    Role.SuperAdmin
                }, save, StatusEnum.UserIsNull);
            return updateStatus;
        }

        public async Task<MethodStatus<Facility>> FacilityUpdateAsync(FacilityInputViewModel model, bool save)
        {
            if (model == null)
                return new MethodStatus<Facility>(StatusEnum.ModelIsNull, null);

            if (model.IsNew)
                return new MethodStatus<Facility>(StatusEnum.IdIsNull, null);

            var entity = await FacilityEntityAsync(model.Id);
            var updateStatus = await _baseService.UpdateAsync(entity,
                _ => entity.Name = model.Name,
                new[]
                {
                    Role.SuperAdmin
                }, save, StatusEnum.UserIsNull);
            return updateStatus;
        }

        public async Task<MethodStatus<Category>> CategoryUpdateAsync(CategoryInputViewModel model, bool save)
        {
            if (model == null)
                return new MethodStatus<Category>(StatusEnum.ModelIsNull, null);

            if (model.IsNew)
                return new MethodStatus<Category>(StatusEnum.IdIsNull, null);

            var entity = await CategoryEntityAsync(model.Id);
            var updateStatus = await _baseService.UpdateAsync(entity,
                _ =>
                {
                    entity.Type = model.Type;
                    entity.Name = model.Name;
                }, new[]
                {
                    Role.SuperAdmin
                }, save, StatusEnum.UserIsNull);
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
            }, save);
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
            }, save);
            return addStatus;
        }

        public async Task<MethodStatus<Category>> CategoryAddAsync(CategoryInputViewModel model, bool save)
        {
            if (model == null)
                return new MethodStatus<Category>(StatusEnum.ModelIsNull, null);

            var category = await _categories.FirstOrDefaultAsync(x => x.Name.Equals(model.Name));
            if (category != null)
                return new MethodStatus<Category>(StatusEnum.AlreadyExists, null);

            var addStatus = await _baseService.AddAsync(new Category
            {
                Name = model.Name,
                Type = model.Type,
            }, new[]
            {
                Role.SuperAdmin, Role.Admin
            }, save);
            return addStatus;
        }

        public async Task<PaginationViewModel<FeatureViewModel>> FeatureListAsync(FeatureSearchViewModel searchModel)
        {
            var query = _baseService.CheckDeletedItemsPrevillege(_features, searchModel, out var currentUser);
            if (query == null)
                return new PaginationViewModel<FeatureViewModel>();

            query = query.AsNoTracking();

            if (searchModel != null)
            {
                if (!string.IsNullOrEmpty(searchModel.Name))
                    query = query.Where(x => EF.Functions.Like(x.Name, searchModel.Name.Like()));

                if (searchModel.Type != null)
                    query = query.Where(x => x.Type == searchModel.Type);

                query = _baseService.AdminSeachConditions(query, searchModel);
            }

            var result = await _baseService.PaginateAsync(query, searchModel,
                item => item.Map<FeatureViewModel>());

            return result;
        }

        public async Task<PaginationViewModel<FacilityViewModel>> FacilityListAsync(FacilitySearchViewModel searchModel)
        {
            var query = _baseService.CheckDeletedItemsPrevillege(_facilities, searchModel, out var currentUser);
            if (query == null)
                return new PaginationViewModel<FacilityViewModel>();

            query = query.AsNoTracking();

            if (searchModel != null)
            {
                if (!string.IsNullOrEmpty(searchModel.Name))
                    query = query.Where(x => EF.Functions.Like(x.Name, searchModel.Name.Like()));

                query = _baseService.AdminSeachConditions(query, searchModel);
            }
            var result = await _baseService.PaginateAsync(query, searchModel,
                item => item.Map<FacilityViewModel>());

            return result;
        }

        public async Task<List<FeatureJsonViewModel>> FeatureListJsonAsync(FeatureTypeEnum type)
        {
            var models = await (from feature in _features
                                where feature.Type == type
                                select feature).ToListAsync();
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
            var facilities = await _facilities.Cacheable().ToListAsync();
            return facilities.Map<Facility, FacilityViewModel>();
        }

        public async Task<List<FeatureViewModel>> FeatureListAsync(params FeatureTypeEnum[] types)
        {
            var query = _features.AsQueryable();

            if (types?.Any() == true)
                query = query.Where(x => types.Contains(x.Type));

            var features = await query.Cacheable().ToListAsync();
            return features.Map<Feature, FeatureViewModel>();
        }

        public async Task<PaginationViewModel<CategoryViewModel>> CategoryListAsync(CategorySearchViewModel searchModel)
        {
            var query = _baseService.CheckDeletedItemsPrevillege(_categories, searchModel, out var currentUser);
            if (query == null)
                return new PaginationViewModel<CategoryViewModel>();

            query = query.AsNoTracking();

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
            var result = await _baseService.PaginateAsync(query, searchModel,
                item => item.Map<CategoryViewModel>());

            return result;
        }

        public async Task<List<CategoryViewModel>> CategoryListAsync()
        {
            var query = _categories.IgnoreQueryFilters();
            var categories = await query.ToListAsync();
            return categories.Map<Category, CategoryViewModel>();
        }

        public async Task<List<CategoryViewModel>> CategoryListAsync(CategoryTypeEnum? category, bool byUserPrevilege)
        {
            var query = _categories.AsQueryable();

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

            var categories = await query
                .Cacheable()
                .ToListAsync()
                ;
            return categories.Map<Category, CategoryViewModel>();
        }
    }
}