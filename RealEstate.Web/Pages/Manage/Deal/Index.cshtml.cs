using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RealEstate.Base;
using RealEstate.Base.Attributes;
using RealEstate.Base.Enums;
using RealEstate.Resources;
using RealEstate.Services.Extensions;
using RealEstate.Services.ServiceLayer;
using RealEstate.Services.ViewModels.ModelBind;
using RealEstate.Services.ViewModels.Search;
using System.Threading.Tasks;

namespace RealEstate.Web.Pages.Manage.Deal
{
    [NavBarHelper(typeof(IndexModel))]
    public class IndexModel : PageModel
    {
        private readonly IDealService _dealService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public IndexModel(
            IDealService dealService,
            IStringLocalizer<SharedResource> sharedLocalizer)
        {
            _dealService = dealService;
            _localizer = sharedLocalizer;
        }

        [BindProperty]
        public DealSearchViewModel SearchInput { get; set; }

        public PaginationViewModel<DealViewModel> List { get; set; }

        public string Status { get; set; }

        public string PageTitle => _localizer["Deals"];

        public async Task OnGetAsync(string pageNo, string status)
        {
            SearchInput = new DealSearchViewModel
            {
                PageNo = pageNo.FixPageNumber(),
            };

            Status = int.TryParse(status, out var statusCode) ? ((StatusEnum)statusCode).GetDisplayName() : null;
            List = await _dealService.ListAsync(SearchInput).ConfigureAwait(false);
        }

        public IActionResult OnPost()
        {
            return RedirectToPage(typeof(IndexModel).Page(), SearchInput.GetSearchParameters());
        }
    }
}