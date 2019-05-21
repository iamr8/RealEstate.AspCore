using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RealEstate.Base.Attributes;
using RealEstate.Base.Enums;
using RealEstate.Resources;
using RealEstate.Services.Extensions;
using RealEstate.Services.ServiceLayer;
using RealEstate.Services.ViewModels.Input;
using System.Threading.Tasks;

namespace RealEstate.Web.Pages.Manage.Category
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    [NavBarHelper(typeof(IndexModel))]
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
        public CategoryInputViewModel NewCategory { get; set; }

        [ViewData]
        public string PageTitle { get; set; }

        public string Status { get; set; }

        public async Task<IActionResult> OnGetAsync(string id, string status)
        {
            if (!string.IsNullOrEmpty(id))
            {
                if (!User.IsInRole(nameof(Role.SuperAdmin)) && !User.IsInRole(nameof(Role.Admin)))
                    return Forbid();

                var model = await _featureService.CategoryInputAsync(id).ConfigureAwait(false);
                if (model == null)
                    return RedirectToPage(typeof(IndexModel).Page());

                NewCategory = model;
                PageTitle = _localizer[SharedResource.EditCategory];
            }
            else
            {
                PageTitle = _localizer[SharedResource.NewCategory];
            }

            Status = !string.IsNullOrEmpty(status)
                ? status
                : null;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var (status, message) = await ModelState.IsValidAsync(
                    () => _featureService.CategoryAddOrUpdateAsync(NewCategory, !NewCategory.IsNew, true)
                    ).ConfigureAwait(false);

            return RedirectToPage(status != StatusEnum.Success
                ? typeof(AddModel).Page()
                : typeof(IndexModel).Page(), new
                {
                    status = message,
                    id = status != StatusEnum.Success ? NewCategory?.Id : null
                });
        }
    }
}