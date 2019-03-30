using RealEstate.Domain;
using RealEstate.Services.Connector;

namespace RealEstate.Services
{
    public interface ILocationService
    {
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
    }
}