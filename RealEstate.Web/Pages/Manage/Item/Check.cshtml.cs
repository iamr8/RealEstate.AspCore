using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RealEstate.Resources;
using RealEstate.Services.ServiceLayer;
using RealEstate.Services.ViewModels.Input;
using System.Threading.Tasks;

namespace RealEstate.Web.Pages.Manage.Item
{
    public class CheckModel : PageModel
    {
        private readonly IItemService _itemService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public CheckModel(
            IItemService itemService,
            IStringLocalizer<SharedResource> localizer)
        {
            _itemService = itemService;
            _localizer = localizer;
        }

        public string PageTitle => _localizer[SharedResource.Check];

        [BindProperty]
        public PropertyCheckViewModel PropertyCheck { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var model = await _itemService.ItemCheckAsync(PropertyCheck);
            if (!model)
                return RedirectToPage(typeof(CheckModel).Page());

            return RedirectToPage(typeof(AddModel).Page());
        }
    }
}