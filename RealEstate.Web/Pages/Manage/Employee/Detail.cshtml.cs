using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RealEstate.Resources;
using RealEstate.Services;
using RealEstate.Services.Extensions;
using RealEstate.Services.ViewModels;
using RealEstate.Services.ViewModels.Input;
using System.Threading.Tasks;
using RealEstate.Services.ServiceLayer;
using RealEstate.Services.ViewModels.ModelBind;

namespace RealEstate.Web.Pages.Manage.Employee
{
    public class DetailModel : PageModel
    {
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly IPaymentService _paymentService;
        private readonly IEmployeeService _employeeService;

        public DetailModel(
            IStringLocalizer<SharedResource> localizer,
            IPaymentService paymentService,
            IEmployeeService employeeService
            )
        {
            _localizer = localizer;
            _employeeService = employeeService;
            _paymentService = paymentService;
        }

        public EmployeeDetailViewModel Detail { get; set; }

        [ViewData]
        public string PageTitle => _localizer[SharedResource.Detail];

        public string Status { get; set; }

        [BindProperty]
        public PaymentInputViewModel NewPayment { get; set; }

        public async Task<IActionResult> OnGetAsync(string id, string status)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToPage(typeof(IndexModel).Page());

            var model = await _employeeService.DetailAsync(id).ConfigureAwait(false);
            if (model == null)
                return RedirectToPage(typeof(IndexModel).Page());

            Detail = model;
            NewPayment = new PaymentInputViewModel
            {
                EmployeeId = Detail.Id
            };
            Status = !string.IsNullOrEmpty(status)
                ? status
                : null;
            return Page();
        }

        public async Task<IActionResult> OnPostPaymentAsync()
        {
            var (status, message) = await ModelState.IsValidAsync(
                () => _paymentService.PaymentAddAsync(NewPayment, true),
                nameof(NewPayment)).ConfigureAwait(false);

            return RedirectToPage(typeof(DetailModel).Page(), new
            {
                id = NewPayment.EmployeeId,
                status = message
            });
        }
    }
}