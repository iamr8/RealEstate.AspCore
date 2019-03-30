using RealEstate.Domain;
using RealEstate.Services.Base;

namespace RealEstate.Services
{
    public interface IDealService
    {
    }

    public class DealService : IDealService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseService _baseService;

        public DealService(
            IUnitOfWork unitOfWork,
            IBaseService baseService
            )
        {
            _unitOfWork = unitOfWork;
            _baseService = baseService;
        }
    }
}