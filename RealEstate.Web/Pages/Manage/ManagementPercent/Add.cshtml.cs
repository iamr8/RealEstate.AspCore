using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RealEstate.Base;
using RealEstate.Base.Attributes;
using RealEstate.Base.Enums;
using RealEstate.Resources;
using RealEstate.Services.ServiceLayer;
using RealEstate.Services.ViewModels.Input;
using System.Threading.Tasks;

namespace RealEstate.Web.Pages.Manage.ManagementPercent
{
    [NavBarHelper(typeof(IndexModel))]
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
        public ManagementPercentInputViewModel NewManagementPercent { get; set; }

        [ViewData]
        public string PageTitle { get; set; }

        [TempData]
        public string ManagementPercentStatus { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                if (!User.IsInRole(nameof(Role.SuperAdmin)) && !User.IsInRole(nameof(Role.Admin)))
                    return Forbid();

                var model = await _paymentService.ManagementPercentInputAsync(id).ConfigureAwait(false);
                if (model == null)
                    return RedirectToPage(typeof(IndexModel).Page());

                NewManagementPercent = model;
                PageTitle = _localizer["EditManagementPercent"];
            }
            else
            {
                PageTitle = _localizer["NewManagementPercent"];
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var finalStatus = ModelState.IsValid
                ? (await _paymentService.ManagementPercentAddOrUpdateAsync(NewManagementPercent, !NewManagementPercent.IsNew, true).ConfigureAwait(false)).Status
                : StatusEnum.RetryAfterReview;

            ManagementPercentStatus = finalStatus.GetDisplayName();
            if (finalStatus != StatusEnum.Success || !NewManagementPercent.IsNew)
                return Page();

            ModelState.Clear();
            NewManagementPercent = default;

            return RedirectToPage(typeof(AddModel).Page(), new
            {
                id = NewManagementPercent?.Id
            });
        }
    }
}