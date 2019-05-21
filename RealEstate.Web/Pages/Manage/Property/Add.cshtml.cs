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

namespace RealEstate.Web.Pages.Manage.Property
{
    [NavBarHelper(typeof(IndexModel))]
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

        public string Status { get; set; }

        public async Task<IActionResult> OnGetAsync(string id, string status)
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

            Status = !string.IsNullOrEmpty(status)
                ? status
                : null;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var (status, message) = await ModelState.IsValidAsync(
                () => _propertyService.PropertyAddOrUpdateAsync(NewProperty, true)
            ).ConfigureAwait(false);

            return RedirectToPage(status != StatusEnum.Success
                ? typeof(AddModel).Page()
                : typeof(IndexModel).Page(), new
                {
                    status = message,
                    id = status != StatusEnum.Success ? NewProperty?.Id : null
                });
        }
    }
}