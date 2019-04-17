using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Resources;
using RealEstate.Services;
using RealEstate.Services.Extensions;
using RealEstate.Services.ViewModels;
using RealEstate.Services.ViewModels.Search;
using System.Threading.Tasks;

namespace RealEstate.Web.Pages.Manage.DealRequest
{
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
        public PropertySearchViewModel SearchInput { get; set; }

        public PaginationViewModel<DealViewModel> List { get; set; }

        public string Status { get; set; }

        public string PageTitle => _localizer["Properties"];

        public async Task OnGetAsync(string pageNo, string status)
        {
            SearchInput = new PropertySearchViewModel
            {
                PageNo = pageNo.FixPageNumber()
            };

            Status = int.TryParse(status, out var statusCode) ? ((StatusEnum)statusCode).GetDisplayName() : null;
            List = await _dealService.RequestListAsync(SearchInput.PageNo).ConfigureAwait(false);
        }

        public IActionResult OnPost()
        {
            return RedirectToPage(typeof(Property.IndexModel).Page(), SearchInput.GetSearchParameters());
        }
    }
}