using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using RealEstate.Base;
using RealEstate.Resources;
using RealEstate.Services.ServiceLayer;
using RealEstate.Services.ViewModels.Input;

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

        [Route("employee/payment/pay")]
        public async Task<IActionResult> PayAsync(string id, string employeeId)
        {
            var (status, newCustomer) = await _paymentService.PayAsync(id, true);
            return RedirectToPage(typeof(Pages.Manage.Employee.DetailModel).Page(), new
            {
                status,
                id = employeeId
            });
        }
    }
}