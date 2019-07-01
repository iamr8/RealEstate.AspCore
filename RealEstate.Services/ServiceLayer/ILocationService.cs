using Microsoft.EntityFrameworkCore;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Services.Database;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using RealEstate.Services.ServiceLayer.Base;
using RealEstate.Services.ViewModels.Input;
using RealEstate.Services.ViewModels.ModelBind;
using RealEstate.Services.ViewModels.Search;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFSecondLevelCache.Core;

namespace RealEstate.Services.ServiceLayer
{
    public interface ILocationService
    {
        Task<MethodStatus<District>> DistrictAddAsync(DistrictInputViewModel model, bool save);

        Task<District> DistrictEntityAsync(string id);

        Task<StatusEnum> DistrictRemoveAsync(string id);

        Task<List<DistrictViewModel>> DistrictListAsync();

        Task<MethodStatus<District>> DistrictAddOrUpdateAsync(DistrictInputViewModel model, bool update, bool save);

        Task<DistrictInputViewModel> DistrictInputAsync(string id);

        Task<MethodStatus<District>> DistrictUpdateAsync(DistrictInputViewModel model, bool save);

        Task<PaginationViewModel<DistrictViewModel>> DistrictListAsync(DistrictSearchViewModel searchModel);
    }

    public class LocationService : ILocationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseService _baseService;
        private readonly DbSet<District> _districts;

        public LocationService(
            IUnitOfWork unitOfWork,
            IBaseService baseService
            )
        {
            _unitOfWork = unitOfWork;
            _baseService = baseService;
            _districts = _unitOfWork.Set<District>();
        }

        public async Task<List<DistrictViewModel>> DistrictListAsync()
        {
            var districts = await _districts.Cacheable().ToListAsync().ConfigureAwait(false);
            return districts.Map<District, DistrictViewModel>();
        }

        public async Task<PaginationViewModel<DistrictViewModel>> DistrictListAsync(DistrictSearchViewModel searchModel)
        {
            var query = _baseService.CheckDeletedItemsPrevillege(_districts, searchModel, out var currentUser);
            if (query == null)
                return new PaginationViewModel<DistrictViewModel>();

            if (searchModel != null)
            {
                if (!string.IsNullOrEmpty(searchModel.Name))
                    query = query.Where(x => EF.Functions.Like(x.Name, searchModel.Name.Like()));

                query = _baseService.AdminSeachConditions(query, searchModel);
            }

            var result = await _baseService.PaginateAsync(query, searchModel,
                item => item.Map<DistrictViewModel>(), currentUser);

            return result;
        }

        public Task<MethodStatus<District>> DistrictAddOrUpdateAsync(DistrictInputViewModel model, bool update, bool save)
        {
            return update
                ? DistrictUpdateAsync(model, save)
                : DistrictAddAsync(model, save);
        }

        public async Task<District> DistrictEntityAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return default;

            var result = await _districts.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            return result;
        }

        public async Task<MethodStatus<District>> DistrictUpdateAsync(DistrictInputViewModel model, bool save)
        {
            if (model == null)
                return new MethodStatus<District>(StatusEnum.ModelIsNull, null);

            if (model.IsNew)
                return new MethodStatus<District>(StatusEnum.IdIsNull, null);

            var entity = await DistrictEntityAsync(model.Id).ConfigureAwait(false);
            var updateStatus = await _baseService.UpdateAsync(entity,
                _ => entity.Name = model.Name,
                new[]
                {
                    Role.SuperAdmin
                }, save, StatusEnum.UserIsNull).ConfigureAwait(false);
            return updateStatus;
        }

        public async Task<StatusEnum> DistrictRemoveAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return StatusEnum.ParamIsNull;

            var entity = await _districts.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            var result = await _baseService.RemoveAsync(entity,
                    new[]
                    {
                        Role.SuperAdmin, Role.Admin
                    })
                .ConfigureAwait(false);

            return result;
        }

        public async Task<MethodStatus<District>> DistrictAddAsync(DistrictInputViewModel model, bool save)
        {
            if (model == null)
                return new MethodStatus<District>(StatusEnum.ModelIsNull, null);

            var addStatus = await _baseService.AddAsync(new District
            {
                Name = model.Name,
            }, new[]
            {
                Role.SuperAdmin, Role.Admin
            }, save).ConfigureAwait(false);
            return addStatus;
        }

        public async Task<DistrictInputViewModel> DistrictInputAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return default;

            var query = _districts.Where(x => x.Id == id)
                .Include(x => x.Properties);

            var model = await query.FirstOrDefaultAsync().ConfigureAwait(false);
            var viewModel = model.Map<DistrictViewModel>();
            if (viewModel == null)
                return default;

            var result = new DistrictInputViewModel
            {
                Id = viewModel.Id,
                Name = viewModel.Name
            };
            return result;
        }
    }
}