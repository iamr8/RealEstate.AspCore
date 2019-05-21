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
using RealEstate.Services.Extensions;

namespace RealEstate.Web.Pages.Manage.District
{
    [NavBarHelper(typeof(IndexModel))]
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

        public string Status { get; set; }

        public async Task<IActionResult> OnGetAsync(string id, string status)
        {
            if (!string.IsNullOrEmpty(id))
            {
                if (!User.IsInRole(nameof(Role.SuperAdmin)) && !User.IsInRole(nameof(Role.Admin)))
                    return Forbid();

                var model = await _locationService.DistrictInputAsync(id).ConfigureAwait(false);
                if (model == null)
                    return RedirectToPage(typeof(IndexModel).Page());

                NewDistrict = model;
                PageTitle = _localizer[SharedResource.EditDistrict];
            }
            else
            {
                PageTitle = _localizer[SharedResource.NewDistrict];
            }

            Status = !string.IsNullOrEmpty(status)
                ? status
                : null;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var (status, message) = await ModelState.IsValidAsync(
                () => _locationService.DistrictAddOrUpdateAsync(NewDistrict, !NewDistrict.IsNew, true)
            ).ConfigureAwait(false);

            return RedirectToPage(status != StatusEnum.Success
                ? typeof(AddModel).Page()
                : typeof(IndexModel).Page(), new
            {
                status = message,
                id = status != StatusEnum.Success ? NewDistrict?.Id : null
            });

        }
    }
}