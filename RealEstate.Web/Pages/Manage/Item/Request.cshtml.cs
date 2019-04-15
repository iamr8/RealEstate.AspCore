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
    public class RequestModel : PageModel
    {
        private readonly IDealService _dealService;
        private readonly IItemService _itemService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public RequestModel(
            IDealService dealService,
            IItemService itemService,
            IStringLocalizer<SharedResource> localizer)
        {
            _dealService = dealService;
            _localizer = localizer;
            _itemService = itemService;
        }

        [BindProperty]
        public ItemRequestInputViewModel NewItemRequest { get; set; }

        public string Status { get; set; }

        public string PageTitle => _localizer["ItemRequest"];

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var model = await _itemService.ItemAsync(id).ConfigureAwait(false);
            if (model?.IsRequested != false)
                return RedirectToPage(typeof(IndexModel).Page());

            NewItemRequest = new ItemRequestInputViewModel
            {
                ItemId = model.Id,
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var finalStatus = ModelState.IsValid
                ? (await _dealService.RequestAsync(NewItemRequest, true).ConfigureAwait(false)).Item1
                : StatusEnum.RetryAfterReview;

            Status = finalStatus.GetDisplayName();
            if (finalStatus != StatusEnum.Success)
                return Page();

            ModelState.Clear();
            NewItemRequest = default;

            return Page();
        }
    }
}