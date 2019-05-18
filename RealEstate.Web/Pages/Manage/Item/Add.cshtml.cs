using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RealEstate.Base.Attributes;
using RealEstate.Base.Enums;
using RealEstate.Resources;
using RealEstate.Services.Extensions;
using RealEstate.Services.ServiceLayer;
using RealEstate.Services.ViewModels.Input;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Web.Pages.Manage.Item
{
    [NavBarHelper(typeof(IndexModel))]
    public class AddModel : PageModel
    {
        private readonly IItemService _itemService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public AddModel(
            IItemService itemService,
            IStringLocalizer<SharedResource> sharedLocalizer
            )
        {
            _itemService = itemService;
            _localizer = sharedLocalizer;
        }

        [BindProperty]
        public ItemInputViewModel NewItem { get; set; }

        [ViewData]
        public string PageTitle { get; set; }

        public StatusEnum Status { get; set; }

        public async Task<IActionResult> OnGetAsync(string id, string status)
        {
            ItemInputViewModel model = null;
            if (!string.IsNullOrEmpty(id))
            {
                if (!User.IsInRole(nameof(Role.SuperAdmin)) && !User.IsInRole(nameof(Role.Admin)))
                    return Forbid();

                model = await _itemService.ItemInputAsync(id).ConfigureAwait(false);
            }

            PageTitle = _localizer[(model == null ? "New" : "Edit") + GetType().Namespaces().Last()];
            NewItem = model;
            Status = !string.IsNullOrEmpty(status) && int.TryParse(status, out var statusInt)
                ? (StatusEnum)statusInt
                : StatusEnum.Ready;

            if (!string.IsNullOrEmpty(id) && model == null)
                return RedirectToPage(typeof(IndexModel).Page());

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var (status, message) = await ModelState.IsValidAsync(
                    () => _itemService.ItemAddOrUpdateAsync(NewItem, !NewItem.IsNew, true))
                .ConfigureAwait(false);

            return RedirectToPage(status != StatusEnum.Success
                ? typeof(AddModel).Page()
                : typeof(IndexModel).Page(), new
                {
                    status = message,
                    id = status != StatusEnum.Success ? NewItem.Id : null
                });
        }
    }
}