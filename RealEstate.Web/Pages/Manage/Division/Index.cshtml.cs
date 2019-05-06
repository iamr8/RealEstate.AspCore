using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RealEstate.Base;
using RealEstate.Resources;
using RealEstate.Services;
using RealEstate.Services.Extensions;
using RealEstate.Services.ViewModels;
using RealEstate.Services.ViewModels.Search;
using System.Threading.Tasks;

namespace RealEstate.Web.Pages.Manage.Division
{
    public class IndexModel : PageModel
    {
        private readonly IDivisionService _divisionService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public IndexModel(
            IDivisionService divisionService,
            IStringLocalizer<SharedResource> sharedLocalizer)
        {
            _divisionService = divisionService;
            _localizer = sharedLocalizer;
        }

        [BindProperty]
        public DivisionSearchViewModel SearchInput { get; set; }

        public PaginationViewModel<DivisionViewModel> List { get; set; }

        [ViewData]
        public string PageTitle => _localizer["Divisions"];

        public async Task OnGetAsync(string pageNo, string divisionName, string id, bool deleted, string dateFrom, string dateTo, string creatorId)
        {
            SearchInput = new DivisionSearchViewModel
            {
                PageNo = pageNo.FixPageNumber(),
                Name = divisionName,
                Id = id,
                IncludeDeletedItems = deleted,
                CreatorId = creatorId,
                CreationDateFrom = dateFrom,
                CreationDateTo = dateTo
            };

            List = await _divisionService.ListAsync(SearchInput).ConfigureAwait(false);
        }

        public IActionResult OnPost()
        {
            return RedirectToPage(typeof(IndexModel).Page(), SearchInput.GetSearchParameters());
        }
    }
}