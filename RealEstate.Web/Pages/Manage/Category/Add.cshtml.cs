using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RealEstate.Base.Enums;
using RealEstate.Extensions;
using RealEstate.Resources;
using RealEstate.Services;
using RealEstate.ViewModels.Input;
using System.Threading.Tasks;

namespace RealEstate.Web.Pages.Manage.Category
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
        public CategoryInputViewModel NewCategory { get; set; }

        [ViewData]
        public string PageTitle { get; set; }

        [TempData]
        public string CategoryStatus { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                if (!User.IsInRole(nameof(Role.SuperAdmin)) && !User.IsInRole(nameof(Role.Admin)))
                    return Forbid();

                var model = await _featureService.CategoryInputAsync(id).ConfigureAwait(false);
                if (model == null)
                    return RedirectToPage(typeof(IndexModel).Page());

                NewCategory = model;
                PageTitle = _localizer["EditCategory"];
            }
            else
            {
                PageTitle = _localizer["NewCategory"];
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var finalStatus = ModelState.IsValid
                ? (await _featureService.CategoryAddOrUpdateAsync(NewCategory, !NewCategory.IsNew, true).ConfigureAwait(false)).Item1
                : StatusEnum.RetryAfterReview;

            CategoryStatus = finalStatus.Display();
            if (finalStatus != StatusEnum.Success || !NewCategory.IsNew)
                return Page();

            ModelState.Clear();
            NewCategory = default;

            return RedirectToPage(typeof(AddModel).Page(), new
            {
                id = NewCategory?.Id
            });
        }
    }
}