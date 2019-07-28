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

namespace RealEstate.Web.Pages.Manage.Item
{
    [NavBarHelper(typeof(IndexModel))]
    public class RequestModel : PageModel
    {
        private readonly IItemService _itemService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public RequestModel(
            IItemService itemService,
            IStringLocalizer<SharedResource> localizer)
        {
            _localizer = localizer;
            _itemService = itemService;
        }

        [BindProperty]
        public DealRequestInputViewModel NewDealRequest { get; set; }

        public string Status { get; set; }

        public string PageTitle => _localizer[SharedResource.DealRequest];

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var model = await _itemService.ItemAsync(id, DealStatusEnum.Rejected);
            if (model?.LastState != DealStatusEnum.Rejected)
                return RedirectToPage(typeof(IndexModel).Page());

            NewDealRequest = new DealRequestInputViewModel
            {
                Id = model.Id,
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var finalStatus = ModelState.IsValid
                ? (await _itemService.RequestAsync(NewDealRequest, true).ConfigureAwait(false)).Status
                : StatusEnum.RetryAfterReview;

            Status = finalStatus.GetDisplayName();
            if (finalStatus != StatusEnum.Success)
                return RedirectToPage(typeof(RequestModel).Page());

            ModelState.Clear();
            NewDealRequest = default;

            return RedirectToPage(typeof(IndexModel).Page());
        }
    }
}