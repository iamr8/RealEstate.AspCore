using Microsoft.EntityFrameworkCore;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Domain;
using RealEstate.Domain.Tables;
using RealEstate.Extensions;
using RealEstate.Services.Base;
using RealEstate.ViewModels;
using RealEstate.ViewModels.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Services
{
    public interface IFeatureService
    {
        Task<PaginationViewModel<CategoryViewModel>> CategoryListAsync(int page, string categoryName, string categoryId, CategoryTypeEnum? type);

        Task<CategoryInputViewModel> CategoryInputAsync(string id);

        Task<StatusEnum> CategoryRemoveAsync(string id);

        Task<(StatusEnum, Category)> CategoryAddAsync(CategoryInputViewModel model, bool save);

        Task<(StatusEnum, Category)> CategoryUpdateAsync(CategoryInputViewModel model, bool save);

        Task<Category> CategoryEntityAsync(string id);

        Task<List<CategoryViewModel>> CategoryListAsync(CategoryTypeEnum? category);
    }

    public class FeatureService : IFeatureService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseService _baseService;
        private readonly IMapService _mapService;
        private readonly DbSet<UserPropertyCategory> _userPropertyCategories;
        private readonly DbSet<UserItemCategory> _userItemCategories;
        private readonly DbSet<Category> _categories;

        public FeatureService(
            IUnitOfWork unitOfWork,
            IBaseService baseService,
            IMapService mapService
            )
        {
            _unitOfWork = unitOfWork;
            _baseService = baseService;
            _mapService = mapService;
            _userPropertyCategories = _unitOfWork.Set<UserPropertyCategory>();
            _userItemCategories = _unitOfWork.Set<UserItemCategory>();
            _categories = _unitOfWork.Set<Category>();
        }

        public async Task<CategoryInputViewModel> CategoryInputAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return default;

            var query = _categories.Where(x => x.Id == id);
            var model = await query.FirstOrDefaultAsync().ConfigureAwait(false);
            var viewModel = _mapService.Map(model);
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

        public async Task<Category> CategoryEntityAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return default;

            var result = await _categories.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            return result;
        }

        public async Task<StatusEnum> CategoryRemoveAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return StatusEnum.ParamIsNull;

            var user = await CategoryEntityAsync(id).ConfigureAwait(false);
            var result = await _baseService.RemoveAsync(user,
                    new[]
                    {
                        Role.SuperAdmin, Role.Admin
                    },
                    true,
                    true)
                .ConfigureAwait(false);

            return result;
        }

        public async Task<(StatusEnum, Category)> CategoryUpdateAsync(CategoryInputViewModel model, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Category>(StatusEnum.ModelIsNull, null);

            if (model.IsNew)
                return new ValueTuple<StatusEnum, Category>(StatusEnum.IdIsNull, null);

            var entity = await CategoryEntityAsync(model.Id).ConfigureAwait(false);
            var updateStatus = await _baseService.UpdateAsync(entity,
                () =>
                {
                    entity.Type = model.Type;
                    entity.Name = model.Name;
                }, new[]
                {
                    Role.SuperAdmin
                }, save, StatusEnum.UserIsNull).ConfigureAwait(false);
            return updateStatus;
        }

        public async Task<(StatusEnum, Category)> CategoryAddAsync(CategoryInputViewModel model, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Category>(StatusEnum.ModelIsNull, null);

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

        public async Task<PaginationViewModel<CategoryViewModel>> CategoryListAsync(int page, string categoryName, string categoryId, CategoryTypeEnum? type)
        {
            var models = _categories as IQueryable<Category>;
            models = models.Include(x => x.Properties);
            models = models.Include(x => x.UserItemCategories);
            models = models.Include(x => x.UserPropertyCategories);
            models = models.Include(x => x.Items);
            models = models.Include(x => x.Logs);

            if (!string.IsNullOrEmpty(categoryName))
                models = models.Where(x => EF.Functions.Like(x.Name, categoryName.LikeExpression()));

            if (!string.IsNullOrEmpty(categoryId))
                models = models.Where(x => EF.Functions.Like(x.Name, categoryId.LikeExpression()));

            if (!string.IsNullOrEmpty(categoryName))
                models = models.Where(x => EF.Functions.Like(x.Name, categoryName.LikeExpression()));

            if (type != null)
                models = models.Where(x => x.Type == type);

            var result = await _baseService.PaginateAsync(models, page, _mapService.Map,
                new[]
                {
                    Role.Admin, Role.SuperAdmin
                }
            ).ConfigureAwait(false);

            return result;
        }

        public async Task<List<CategoryViewModel>> CategoryListAsync(CategoryTypeEnum? category)
        {
            var query = _categories as IQueryable<Category>;
            query = query.Filtered();

            if (category != null)
                query = query.Where(x => x.Type == category);

            var categories = await query.ToListAsync().ConfigureAwait(false);
            return _mapService.Map(categories);
        }
    }
}