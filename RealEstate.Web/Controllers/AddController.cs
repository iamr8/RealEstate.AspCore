using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using RealEstate.Base;
using RealEstate.Resources;
using RealEstate.Services;
using RealEstate.Services.ViewModels.Input;
using System.Threading.Tasks;

namespace RealEstate.Web.Controllers
{
    [Route("manage")]
    [ApiController]
    public class AddController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IPropertyService _propertyService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public AddController(
            ICustomerService customerService,
            IPropertyService propertyService,
            IStringLocalizer<SharedResource> localizer)
        {
            _customerService = customerService;
            _propertyService = propertyService;
            _localizer = localizer;
        }

        [Route("customer/addItem"), HttpPost]
        public async Task<IActionResult> CustomerAsync([FromForm] CustomerInputViewModel model)
        {
            var (status, newCustomer) = await _customerService.CustomerAddAsync(model, false, true).ConfigureAwait(false);
            return new JsonResult(new JsonStatusValueViewModel
            {
                Status = status,
                Id = newCustomer?.Id,
                Name = newCustomer != null ? $"{newCustomer.Name} • {newCustomer.MobileNumber}" : null
            });
        }

        [Route("property/addItem"), HttpPost]
        public async Task<IActionResult> PropertyAsync([FromForm] PropertyInputViewModel model)
        {
            var (status, newProperty) = await _propertyService.PropertyAddOrUpdateAsync(model, true).ConfigureAwait(false);
            return new JsonResult(new JsonStatusViewModel
            {
                Status = status,
                Id = newProperty?.Id,
            });
        }
    }
}