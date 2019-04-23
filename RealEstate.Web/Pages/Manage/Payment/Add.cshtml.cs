using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Resources;
using RealEstate.Services;
using RealEstate.Services.ViewModels.Input;
using System.Threading.Tasks;

namespace RealEstate.Web.Pages.Manage.Payment
{
    public class AddModel : PageModel
    {
        private readonly IPaymentService _paymentService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public AddModel(
            IPaymentService paymentService,
            IStringLocalizer<SharedResource> sharedLocalizer
            )
        {
            _paymentService = paymentService;
            _localizer = sharedLocalizer;
        }

        [BindProperty]
        public PaymentInputViewModel NewPayment { get; set; }

        [ViewData]
        public string PageTitle { get; set; }

        [TempData]
        public string PaymentStatus { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            PageTitle = _localizer["NewPayment"];
            NewPayment = new PaymentInputViewModel();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var finalStatus = ModelState.IsValid
                ? (await _paymentService.PaymentAddAsync(NewPayment, true, PaymentTypeEnum.Salary).ConfigureAwait(false)).Item1
                : StatusEnum.RetryAfterReview;

            PaymentStatus = finalStatus.GetDisplayName();
            if (finalStatus != StatusEnum.Success)
                return Page();

            ModelState.Clear();
            NewPayment = default;

            return RedirectToPage(typeof(AddModel).Page());
        }
    }
}