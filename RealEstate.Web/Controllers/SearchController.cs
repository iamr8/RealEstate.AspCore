using Microsoft.AspNetCore.Mvc;
using RealEstate.Services;
using System.Threading.Tasks;

namespace RealEstate.Web.Controllers
{
    [Route("manage")]
    public class SearchController : Controller
    {
        private readonly IPropertyService _propertyService;
        private readonly ICustomerService _customerService;
        private readonly IItemService _itemService;

        public SearchController(
            IPropertyService propertyService,
            ICustomerService customerService,
            IItemService itemService
            )
        {
            _propertyService = propertyService;
            _customerService = customerService;
            _itemService = itemService;
        }

        //[Route("property/search")]
        //public async Task<IActionResult> PropertyAsync(string term)
        //{
        //    var model = await _propertyService.PropertyListAsync(term).ConfigureAwait(false);
        //    return new JsonResult(model);
        //}

        [Route("item/search")]
        public async Task<IActionResult> ItemAsync(string district, string category, string street)
        {
            var model = await _itemService.ItemListAsync(district, category, street).ConfigureAwait(false);
            return new JsonResult(model);
        }

        [Route("customer/search")]
        public async Task<IActionResult> CustomerAsync(string name, string mobile)
        {
            var model = await _customerService.CustomerListAsync(name, mobile).ConfigureAwait(false);
            return new JsonResult(model);
        }
    }
}