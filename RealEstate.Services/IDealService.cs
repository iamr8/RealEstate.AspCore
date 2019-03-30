using RealEstate.Domain;
using RealEstate.Services.Connector;

namespace RealEstate.Services
{
    public interface IDealService
    {
    }

    public class DealService : IDealService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseService _baseService;
        private readonly IItemService _itemService;
        private readonly IPaymentService _paymentService;
        private readonly IContactService _contactService;

        public DealService(
            IUnitOfWork unitOfWork,
            IBaseService baseService,
            IContactService contactService,
            IItemService itemService,
            IPaymentService paymentService
            )
        {
            _unitOfWork = unitOfWork;
            _itemService = itemService;
            _paymentService = paymentService;
            _contactService = contactService;
            _baseService = baseService;
        }
    }
}