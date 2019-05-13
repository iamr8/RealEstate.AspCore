using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Resources;
using RealEstate.Services;
using RealEstate.Services.ViewModels.Input;
using System.Threading.Tasks;

namespace RealEstate.Web.Pages.Manage.Property
{
    public class AddModel : PageModel
    {
        private readonly IPropertyService _propertyService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public AddModel(
            IPropertyService propertyService,
            IStringLocalizer<SharedResource> sharedLocalizer
            )
        {
            _propertyService = propertyService;
            _localizer = sharedLocalizer;
        }

        [BindProperty]
        public PropertyInputViewModel NewProperty { get; set; }

        public string PageTitle { get; set; }

        [TempData]
        public string PropertyStatus { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                if (!User.IsInRole(nameof(Role.SuperAdmin)) && !User.IsInRole(nameof(Role.Admin)))
                    return Forbid();

                var model = await _propertyService.PropertyInputAsync(id).ConfigureAwait(false);
                if (model == null)
                    return RedirectToPage(typeof(IndexModel).Page());

                NewProperty = model;
                PageTitle = _localizer["EditProperty"];
            }
            else
            {
                PageTitle = _localizer["NewProperty"];
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var finalStatus = ModelState.IsValid
                ? (await _propertyService.PropertyAddOrUpdateAsync(NewProperty, true).ConfigureAwait(false)).Status
                : StatusEnum.RetryAfterReview;

            PropertyStatus = finalStatus.GetDisplayName();
            if (finalStatus != StatusEnum.Success || !NewProperty.IsNew)
                return Page();

            ModelState.Clear();
            NewProperty = default;

            return RedirectToPage(typeof(Applicant.AddModel).Page(), new
            {
                id = NewProperty?.Id
            });
        }
    }
}