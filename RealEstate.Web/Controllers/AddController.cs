using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using RealEstate.Base;
using RealEstate.Resources;
using RealEstate.Services.ServiceLayer;
using RealEstate.Services.ViewModels.Input;
using System.Threading.Tasks;
using RealEstate.Services.Extensions;

namespace RealEstate.Web.Controllers
{
    [Route("manage")]
    [ApiController]
    public class AddController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IPaymentService _paymentService;
        private readonly IPropertyService _propertyService;
        private readonly IItemService _itemService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public AddController(
            ICustomerService customerService,
            IPropertyService propertyService,
            IItemService itemService,
            IPaymentService paymentService,
            IStringLocalizer<SharedResource> localizer)
        {
            _customerService = customerService;
            _itemService = itemService;
            _paymentService = paymentService;
            _propertyService = propertyService;
            _localizer = localizer;
        }

        [Route("customer/addItem"), HttpPost]
        public async Task<IActionResult> CustomerAsync([FromForm] CustomerInputViewModel model)
        {
            var (status, newCustomer) = await _customerService.CustomerAddAsync(model, false, true).ConfigureAwait(false);
            return new JsonResult(new JsonStatusViewModel
            {
                StatusCode = (int)status,
                Id = newCustomer?.Id,
                Message = status.GetDisplayName()
            });
        }

        [Route("employee/payment/pay")]
        public async Task<IActionResult> PayAsync(string id, string employeeId)
        {
            var (status, newCustomer) = await _paymentService.PayAsync(id, true).ConfigureAwait(false);
            return RedirectToPage(typeof(Pages.Manage.Employee.DetailModel).Page(), new
            {
                status,
                id = employeeId
            });
        }
        [Route("item/addItem"), HttpPost]
        public async Task<IActionResult> ItemAsync([FromForm] PropertyInputViewModel model)
        {
            var (status, newProperty) = await _propertyService.PropertyAddOrUpdateAsync(model, true).ConfigureAwait(false);
            return new JsonResult(new JsonStatusViewModel
            {
                StatusCode = (int)status,
                Id = newProperty?.Id,
                Message = status.GetDisplayName()
            });
        }
        [Route("property/addItem"), HttpPost]
        public async Task<IActionResult> PropertyAsync([FromForm] PropertyInputViewModel model)
        {
            var (status, newProperty) = await _propertyService.PropertyAddOrUpdateAsync(model, true).ConfigureAwait(false);
            return new JsonResult(new JsonStatusViewModel
            {
                StatusCode = (int)status,
                Id = newProperty?.Id,
                Message = status.GetDisplayName()
            });
        }
    }
}