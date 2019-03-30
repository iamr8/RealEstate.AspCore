using RealEstate.Domain;
using RealEstate.Services.Connector;

namespace RealEstate.Services
{
    public interface IItemService
    {
    }

    public class ItemService : IItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFeatureService _featureService;
        private readonly IPropertyService _propertyService;
        private readonly IContactService _contactService;
        private readonly IBaseService _baseService;

        public ItemService(
            IBaseService baseService,
            IUnitOfWork unitOfWork,
            IFeatureService featureService,
            IPropertyService propertyService,
            IContactService contactService
            )
        {
            _baseService = baseService;
            _unitOfWork = unitOfWork;
            _contactService = contactService;
            _propertyService = propertyService;
            _featureService = featureService;
        }
    }
}