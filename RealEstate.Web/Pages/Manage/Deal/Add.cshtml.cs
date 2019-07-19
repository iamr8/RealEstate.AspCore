using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RealEstate.Base.Attributes;
using RealEstate.Base.Enums;
using RealEstate.Resources;
using RealEstate.Services.Extensions;
using RealEstate.Services.ServiceLayer;
using RealEstate.Services.ViewModels.Input;
using RealEstate.Services.ViewModels.ModelBind;
using System.Threading.Tasks;

namespace RealEstate.Web.Pages.Manage.Deal
{
    [NavBarHelper(typeof(IndexModel))]
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

        public string Status { get; set; }

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

                var model = await _dealService.DealInputAsync(id);
                NewDeal = model;

                var info = await _itemService.ItemAsync(id, null);
                if (info == null)
                    return RedirectToPage(typeof(Deal.IndexModel).Page());

                ItemInfo = info;
                PageTitle = model == null ? _localizer["NewDeal"] : _localizer["EditDeal"];
            }

            Status = !string.IsNullOrEmpty(status)
                ? status
                : null;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var (status, message) = await ModelState.IsValidAsync(
                () => _dealService.AddOrUpdateAsync(NewDeal, !NewDeal.IsNew, true)
            );

            return RedirectToPage(status != StatusEnum.Success
                ? typeof(AddModel).Page()
                : typeof(IndexModel).Page(), new
                {
                    status = message,
                    id = status != StatusEnum.Success ? NewDeal?.Id : null
                });
        }
    }
}