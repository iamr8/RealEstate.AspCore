using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Resources;
using RealEstate.Services;
using RealEstate.Services.ViewModels.Input;
using System.Threading.Tasks;

namespace RealEstate.Web.Pages.Manage.Facility
{
    public class AddModel : PageModel
    {
        private readonly IFeatureService _featureService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public AddModel(
            IFeatureService featureService,
            IStringLocalizer<SharedResource> sharedLocalizer
            )
        {
            _featureService = featureService;
            _localizer = sharedLocalizer;
        }

        [BindProperty]
        public FacilityInputViewModel NewFacility { get; set; }

        [ViewData]
        public string PageTitle { get; set; }

        [TempData]
        public string FacilityStatus { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                if (!User.IsInRole(nameof(Role.SuperAdmin)) && !User.IsInRole(nameof(Role.Admin)))
                    return Forbid();

                var model = await _featureService.FacilityInputAsync(id).ConfigureAwait(false);
                if (model == null)
                    return RedirectToPage(typeof(Facility.IndexModel).Page());

                NewFacility = model;
                PageTitle = _localizer["EditFacility"];
            }
            else
            {
                PageTitle = _localizer["NewFacility"];
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var finalStatus = ModelState.IsValid
                ? (await _featureService.FacilityAddOrUpdateAsync(NewFacility, !NewFacility.IsNew, true).ConfigureAwait(false)).Status
                : StatusEnum.RetryAfterReview;

            FacilityStatus = finalStatus.GetDisplayName();
            if (finalStatus != StatusEnum.Success || !NewFacility.IsNew)
                return Page();

            ModelState.Clear();
            NewFacility = default;

            return RedirectToPage(typeof(AddModel).Page(), new
            {
                id = NewFacility?.Id
            });
        }
    }
}