using RealEstate.Domain;
using RealEstate.Services.Base;

namespace RealEstate.Services
{
    public interface IItemService
    {
    }

    public class ItemService : IItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseService _baseService;

        public ItemService(
            IBaseService baseService,
            IUnitOfWork unitOfWork
            )
        {
            _baseService = baseService;
            _unitOfWork = unitOfWork;
        }
    }
}