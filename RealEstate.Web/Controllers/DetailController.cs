using Microsoft.AspNetCore.Mvc;
using RealEstate.Services;
using System.Threading.Tasks;

namespace RealEstate.Web.Controllers
{
    [Route("manage")]
    [ApiController]
    public class DetailController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IPropertyService _propertyService;

        public DetailController(
            ICustomerService customerService,
            IPropertyService propertyService
            )
        {
            _customerService = customerService;
            _propertyService = propertyService;
        }

        [Route("customer/detail")]
        public async Task<IActionResult> CustomerAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return new JsonResult(null);

            var models = await _customerService.CustomerJsonAsync(id).ConfigureAwait(false);
            return new JsonResult(models);
        }

        [Route("ownership/detail")]
        public async Task<IActionResult> OwnershipAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return new JsonResult(null);

            var models = await _customerService.OwnershipJsonAsync(id).ConfigureAwait(false);
            return new JsonResult(models);
        }

        [Route("property/detail")]
        public async Task<IActionResult> PropertyAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return new JsonResult(null);

            var models = await _propertyService.PropertyJsonAsync(id).ConfigureAwait(false);
            return new JsonResult(models);
        }
    }
}