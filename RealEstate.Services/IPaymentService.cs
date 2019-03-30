using RealEstate.Domain;
using RealEstate.Services.Base;

namespace RealEstate.Services
{
    public interface IPaymentService
    {
    }

    public class PaymentService : IPaymentService
    {
        private readonly IBaseService _baseService;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(
            IBaseService baseService,
            IUnitOfWork unitOfWork
            )
        {
            _baseService = baseService;
            _unitOfWork = unitOfWork;
        }
    }
}