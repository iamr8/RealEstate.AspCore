using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Resources;
using RealEstate.Services;
using RealEstate.Services.ViewModels;
using RealEstate.Services.ViewModels.Input;
using System.Threading.Tasks;
using RealEstate.Services.ServiceLayer;
using RealEstate.Services.ViewModels.ModelBind;

namespace RealEstate.Web.Pages.Manage.Deal
{
    public class AddModel : PageModel
    {
        private readonly IDealService _dealService;
        private readonly IItemService _itemService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public AddModel(
            IDealService dealService,
            IItemService itemService,
            IStringLocalizer<SharedResource> sharedLocalizer
            )
        {
            _dealService = dealService;
            _itemService = itemService;
            _localizer = sharedLocalizer;
        }

        [BindProperty]
        public DealInputViewModel NewDeal { get; set; }

        public ItemViewModel ItemInfo { get; set; }

        [ViewData]
        public string PageTitle { get; set; }

        public string DealStatus { get; set; }

        public async Task<IActionResult> OnGetAsync(string id, string status)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToPage(typeof(IndexModel).Page());
            }
            else
            {
                if (!User.IsInRole(nameof(Role.SuperAdmin)) && !User.IsInRole(nameof(Role.Admin)))
                    return Forbid();

                var model = await _dealService.DealInputAsync(id).ConfigureAwait(false);
                NewDeal = model;

                var info = await _itemService.ItemAsync(id, null).ConfigureAwait(false);
                if (info == null)
                    return RedirectToPage(typeof(Deal.IndexModel).Page());

                ItemInfo = info;
                PageTitle = model == null ? _localizer["NewDeal"] : _localizer["EditDeal"];
            }

            DealStatus = !string.IsNullOrEmpty(status) ? status.To<StatusEnum>().GetDisplayName() : null;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var finalStatus = ModelState.IsValid
                ? (await _dealService.AddOrUpdateAsync(NewDeal, !NewDeal.IsNew, true).ConfigureAwait(false)).Status
                : StatusEnum.RetryAfterReview;

            DealStatus = finalStatus.GetDisplayName();
            if (finalStatus != StatusEnum.Success || !NewDeal.IsNew)
                return RedirectToPage(typeof(Deal.AddModel).Page(), new
                {
                    id = NewDeal?.Id,
                    status = finalStatus
                });

            ModelState.Clear();
            NewDeal = default;

            return RedirectToPage(typeof(Deal.AddModel).Page(), new
            {
                id = NewDeal?.Id,
                status = StatusEnum.Success
            });
        }
    }
}