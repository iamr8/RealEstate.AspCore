using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RealEstate.Base;
using RealEstate.Resources;
using RealEstate.Services.Extensions;
using RealEstate.Services.ServiceLayer;
using RealEstate.Services.ViewModels.ModelBind;
using RealEstate.Services.ViewModels.Search;
using System.Threading.Tasks;

namespace RealEstate.Web.Pages.Manage.DealRequest
{
    public class IndexModel : PageModel
    {
        private readonly IItemService _itemService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public IndexModel(
            IItemService itemService,
            IStringLocalizer<SharedResource> sharedLocalizer)
        {
            _itemService = itemService;
            _localizer = sharedLocalizer;
        }

        [BindProperty]
        public DealRequestSearchViewModel SearchInput { get; set; }

        public PaginationViewModel<ItemViewModel> List { get; set; }

        public string Status { get; set; }

        public string PageTitle => _localizer[SharedResource.DealRequests];

        public async Task OnGetAsync(string pageNo, string status)
        {
            SearchInput = new DealRequestSearchViewModel
            {
                PageNo = pageNo.FixPageNumber()
            };

            Status = !string.IsNullOrEmpty(status)
                ? status
                : null;
            List = await _itemService.RequestListAsync(SearchInput);
        }

        public IActionResult OnPost()
        {
            return RedirectToPage(typeof(IndexModel).Page(), SearchInput.RouteDictionary());
        }
    }
}