using RealEstate.Base.Enums;
using RealEstate.Domain;
using RealEstate.Domain.Tables;
using RealEstate.Services.Base;
using RealEstate.ViewModels.Input;
using System;
using System.Threading.Tasks;

namespace RealEstate.Services
{
    public interface ILocationService
    {
        Task<(StatusEnum, District)> DistrictAddAsync(DistrictInputViewModel model, bool save);
    }

    public class LocationService : ILocationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseService _baseService;

        public LocationService(
            IUnitOfWork unitOfWork,
            IBaseService baseService
            )
        {
            _unitOfWork = unitOfWork;
            _baseService = baseService;
        }

        public async Task<(StatusEnum, District)> DistrictAddAsync(DistrictInputViewModel model, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, District>(StatusEnum.ModelIsNull, null);

            var addStatus = await _baseService.AddAsync(
                new District(),
                new[]
                {
                    Role.SuperAdmin, Role.Admin
                },
                save).ConfigureAwait(false);
            return addStatus;
        }
    }
}