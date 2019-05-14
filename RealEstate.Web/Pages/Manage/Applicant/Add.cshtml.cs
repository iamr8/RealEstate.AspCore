using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Localization;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Resources;
using RealEstate.Services;
using RealEstate.Services.ViewModels.Input;
using System.Threading.Tasks;
using RealEstate.Services.ServiceLayer;

namespace RealEstate.Web.Pages.Manage.Applicant
{
    public class AddModel : PageModel
    {
        private readonly ICustomerService _customerService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public AddModel(
            ICustomerService customerService,
            IStringLocalizer<SharedResource> sharedLocalizer
            )
        {
            _customerService = customerService;
            _localizer = sharedLocalizer;
        }

        [BindProperty]
        public ApplicantInputViewModel NewApplicant { get; set; }

        [ViewData]
        public string PageTitle { get; set; }

        [TempData]
        public string ApplicantStatus { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                if (!User.IsInRole(nameof(Role.SuperAdmin)) && !User.IsInRole(nameof(Role.Admin)))
                    return Forbid();

                var model = await _customerService.ApplicantInputAsync(id).ConfigureAwait(false);
                if (model == null)
                    return RedirectToPage(typeof(Applicant.IndexModel).Page());

                NewApplicant = model;
                PageTitle = _localizer["EditApplicant"];
            }
            else
            {
                PageTitle = _localizer["NewApplicant"];
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var finalStatus = ModelState.IsValid
                ? (await _customerService.ApplicantAddOrUpdateAsync(NewApplicant, !NewApplicant.IsNew, true).ConfigureAwait(false)).Status
                : StatusEnum.RetryAfterReview;

            ApplicantStatus = finalStatus.GetDisplayName();
            if (finalStatus != StatusEnum.Success || !NewApplicant.IsNew)
                return Page();

            ModelState.Clear();
            NewApplicant = default;

            return RedirectToPage(typeof(Applicant.AddModel).Page(), new
            {
                id = NewApplicant?.Id
            });
        }
    }
}