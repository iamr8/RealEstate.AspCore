using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Resources;
using RealEstate.Services;
using RealEstate.Services.ViewModels.Input;
using System.Threading.Tasks;
using RealEstate.Services.ServiceLayer;

namespace RealEstate.Web.Pages.Manage.Feature
{
    [Authorize(Roles = "Admin,SuperAdmin")]
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
        public FeatureInputViewModel NewFeature { get; set; }

        [ViewData]
        public string PageTitle { get; set; }

        [TempData]
        public string FeatureStatus { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                if (!User.IsInRole(nameof(Role.SuperAdmin)) && !User.IsInRole(nameof(Role.Admin)))
                    return Forbid();

                var model = await _featureService.FeatureInputAsync(id).ConfigureAwait(false);
                if (model == null)
                    return RedirectToPage(typeof(IndexModel).Page());

                NewFeature = model;
                PageTitle = _localizer["EditFeature"];
            }
            else
            {
                PageTitle = _localizer["NewFeature"];
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var finalStatus = ModelState.IsValid
                ? (await _featureService.FeatureAddOrUpdateAsync(NewFeature, !NewFeature.IsNew, true).ConfigureAwait(false)).Status
                : StatusEnum.RetryAfterReview;

            FeatureStatus = finalStatus.GetDisplayName();
            if (finalStatus != StatusEnum.Success || !NewFeature.IsNew)
                return Page();

            ModelState.Clear();
            NewFeature = default;

            return RedirectToPage(typeof(AddModel).Page(), new
            {
                id = NewFeature?.Id
            });
        }
    }
}