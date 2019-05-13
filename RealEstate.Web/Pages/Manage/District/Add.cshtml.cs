using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Resources;
using RealEstate.Services;
using RealEstate.Services.ViewModels.Input;
using System.Threading.Tasks;

namespace RealEstate.Web.Pages.Manage.District
{
    public class AddModel : PageModel
    {
        private readonly ILocationService _locationService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public AddModel(
            ILocationService locationService,
            IStringLocalizer<SharedResource> sharedLocalizer
            )
        {
            _locationService = locationService;
            _localizer = sharedLocalizer;
        }

        [BindProperty]
        public DistrictInputViewModel NewDistrict { get; set; }

        [ViewData]
        public string PageTitle { get; set; }

        [TempData]
        public string DistrictStatus { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                if (!User.IsInRole(nameof(Role.SuperAdmin)) && !User.IsInRole(nameof(Role.Admin)))
                    return Forbid();

                var model = await _locationService.DistrictInputAsync(id).ConfigureAwait(false);
                if (model == null)
                    return RedirectToPage(typeof(IndexModel).Page());

                NewDistrict = model;
                PageTitle = _localizer["EditDistrict"];
            }
            else
            {
                PageTitle = _localizer["NewDistrict"];
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var finalStatus = ModelState.IsValid
                ? (await _locationService.DistrictAddOrUpdateAsync(NewDistrict, !NewDistrict.IsNew, true).ConfigureAwait(false)).Status
                : StatusEnum.RetryAfterReview;

            DistrictStatus = finalStatus.GetDisplayName();
            if (finalStatus != StatusEnum.Success || !NewDistrict.IsNew)
                return Page();

            ModelState.Clear();
            NewDistrict = default;

            return RedirectToPage(typeof(AddModel).Page(), new
            {
                id = NewDistrict?.Id
            });
        }
    }
}