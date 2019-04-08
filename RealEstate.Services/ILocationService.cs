using Microsoft.EntityFrameworkCore;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Domain;
using RealEstate.Domain.Tables;
using RealEstate.Extensions;
using RealEstate.Services.Base;
using RealEstate.ViewModels;
using RealEstate.ViewModels.Input;
using RealEstate.ViewModels.Search;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Services
{
    public interface ILocationService
    {
        Task<(StatusEnum, District)> DistrictAddAsync(DistrictInputViewModel model, bool save);

        Task<District> DistrictEntityAsync(string id);

        Task<StatusEnum> DistrictRemoveAsync(string id);

        Task<(StatusEnum, District)> DistrictAddOrUpdateAsync(DistrictInputViewModel model, bool update, bool save);

        Task<DistrictInputViewModel> DistrictInputAsync(string id);

        Task<(StatusEnum, District)> DistrictUpdateAsync(DistrictInputViewModel model, bool save);

        Task<PaginationViewModel<DistrictViewModel>> DistrictListAsync(DistrictSearchViewModel searchModel);
    }

    public class LocationService : ILocationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseService _baseService;
        private readonly IMapService _mapService;
        private readonly DbSet<District> _districts;

        public LocationService(
            IUnitOfWork unitOfWork,
            IBaseService baseService,
            IMapService mapService
            )
        {
            _unitOfWork = unitOfWork;
            _baseService = baseService;
            _mapService = mapService;
            _districts = _unitOfWork.Set<District>();
        }

        public async Task<PaginationViewModel<DistrictViewModel>> DistrictListAsync(DistrictSearchViewModel searchModel)
        {
            var models = _districts as IQueryable<District>;
            models = models.Include(x => x.Properties);
            models = models.Include(x => x.Logs);

            if (searchModel != null)
            {
                if (!string.IsNullOrEmpty(searchModel.Name))
                    models = models.Where(x => EF.Functions.Like(x.Name, searchModel.Name.LikeExpression()));
            }
            var result = await _baseService.PaginateAsync(models, searchModel?.PageNo ?? 1, _mapService.Map,
                new[]
                {
                    Role.Admin, Role.SuperAdmin
                }
            ).ConfigureAwait(false);

            return result;
        }

        public Task<(StatusEnum, District)> DistrictAddOrUpdateAsync(DistrictInputViewModel model, bool update, bool save)
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

        public async Task<(StatusEnum, District)> DistrictUpdateAsync(DistrictInputViewModel model, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, District>(StatusEnum.ModelIsNull, null);

            if (model.IsNew)
                return new ValueTuple<StatusEnum, District>(StatusEnum.IdIsNull, null);

            var entity = await DistrictEntityAsync(model.Id).ConfigureAwait(false);
            var updateStatus = await _baseService.UpdateAsync(entity,
                () =>
                {
                    entity.Name = model.Name;
                }, new[]
                {
                    Role.SuperAdmin
                }, save, StatusEnum.UserIsNull).ConfigureAwait(false);
            return updateStatus;
        }

        public async Task<StatusEnum> DistrictRemoveAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return StatusEnum.ParamIsNull;

            var user = await DistrictEntityAsync(id).ConfigureAwait(false);
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

        public async Task<(StatusEnum, District)> DistrictAddAsync(DistrictInputViewModel model, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, District>(StatusEnum.ModelIsNull, null);

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
            if (string.IsNullOrEmpty(id)) return null;

            var query = _districts.Where(x => x.Id == id)
                .Include(x => x.Properties);

            var model = await query.FirstOrDefaultAsync().ConfigureAwait(false);
            var viewModel = _mapService.Map(model);
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