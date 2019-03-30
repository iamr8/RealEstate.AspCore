using RealEstate.Domain;
using RealEstate.Services.Base;

namespace RealEstate.Services
{
    public interface IFeatureService
    {
    }

    public class FeatureService : IFeatureService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseService _baseService;

        public FeatureService(
            IUnitOfWork unitOfWork,
            IBaseService baseService
            )
        {
            _unitOfWork = unitOfWork;
            _baseService = baseService;
        }
    }
}