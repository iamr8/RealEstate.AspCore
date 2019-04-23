using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Resources;
using RealEstate.Services;
using RealEstate.Services.ViewModels.Input;
using System.Threading.Tasks;

namespace RealEstate.Web.Pages.Manage.Item
{
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

        public string ItemStatus { get; set; }

        public async Task<IActionResult> OnGetAsync(string id, string status)
        {
            if (!string.IsNullOrEmpty(id))
            {
                if (!User.IsInRole(nameof(Role.SuperAdmin)) && !User.IsInRole(nameof(Role.Admin)))
                    return Forbid();

                var model = await _itemService.ItemInputAsync(id).ConfigureAwait(false);
                if (model == null)
                    return RedirectToPage(typeof(IndexModel).Page());

                NewItem = model;
                PageTitle = _localizer["EditItem"];
            }
            else
            {
                PageTitle = _localizer["NewItem"];
            }

            ItemStatus = !string.IsNullOrEmpty(status) ? status.To<StatusEnum>().GetDisplayName() : null;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var finalStatus = ModelState.IsValid
                ? (await _itemService.ItemAddOrUpdateAsync(NewItem, !NewItem.IsNew, true).ConfigureAwait(false)).Item1
                : StatusEnum.RetryAfterReview;

            ItemStatus = finalStatus.GetDisplayName();
            if (finalStatus != StatusEnum.Success || !NewItem.IsNew)
                return RedirectToPage(typeof(AddModel).Page(), new
                {
                    id = NewItem?.Id,
                    status = finalStatus
                });

            ModelState.Clear();
            NewItem = default;

            return RedirectToPage(typeof(AddModel).Page(), new
            {
                id = NewItem?.Id,
                status = StatusEnum.Success
            });
        }
    }
}