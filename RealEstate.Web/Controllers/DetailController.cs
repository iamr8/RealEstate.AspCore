using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.ServiceLayer;
using RealEstate.Services.ViewComponents;

namespace RealEstate.Web.Controllers
{
    [Route("manage")]
    [ApiController]
    public class DetailController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IPropertyService _propertyService;
        private readonly IItemService _itemService;

        public DetailController(
            ICustomerService customerService,
            IPropertyService propertyService,
            IItemService itemService
            )
        {
            _customerService = customerService;
            _propertyService = propertyService;
            _itemService = itemService;
        }

        [Route("property/detail")]
        public async Task<IActionResult> PropertyAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return new JsonResult(null);

            var models = await _propertyService.PropertyJsonAsync(id);
            return new JsonResult(models);
        }

        [Route("item/detail")]
        public IActionResult Item(string id)
        {
            if (string.IsNullOrEmpty(id))
                return new JsonResult(null);

            return new ViewComponentResult
            {
                ViewComponentType = typeof(ItemDetailViewComponent),
                Arguments = new
                {
                    id
                },
            };
        }
    }
}