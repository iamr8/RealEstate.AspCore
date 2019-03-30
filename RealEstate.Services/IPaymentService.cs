using RealEstate.Domain;
using RealEstate.Services.Connector;

namespace RealEstate.Services
{
    public interface IPaymentService
    {
    }

    public class PaymentService : IPaymentService
    {
        private readonly IBaseService _baseService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly IPictureService _pictureService;

        public PaymentService(
            IBaseService baseService,
            IUnitOfWork unitOfWork,
            IPictureService pictureService,
            IUserService userService
            )
        {
            _baseService = baseService;
            _unitOfWork = unitOfWork;
            _userService = userService;
            _pictureService = pictureService;
        }
    }
}