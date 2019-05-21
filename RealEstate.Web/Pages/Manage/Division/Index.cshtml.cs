using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RealEstate.Base;
using RealEstate.Base.Attributes;
using RealEstate.Resources;
using RealEstate.Services.Extensions;
using RealEstate.Services.ServiceLayer;
using RealEstate.Services.ViewModels.ModelBind;
using RealEstate.Services.ViewModels.Search;
using System.Threading.Tasks;

namespace RealEstate.Web.Pages.Manage.Division
{
    [NavBarHelper(typeof(IndexModel))]
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
        public string PageTitle => _localizer[SharedResource.Divisions];

        public string Status { get; set; }

        public async Task OnGetAsync(string pageNo, string divisionName, string id, bool deleted, string dateFrom, string dateTo, string creatorId, string status)
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

            Status = !string.IsNullOrEmpty(status)
                ? status
                : null;
            List = await _divisionService.ListAsync(SearchInput).ConfigureAwait(false);
        }

        public IActionResult OnPost()
        {
            return RedirectToPage(typeof(IndexModel).Page(), SearchInput.GetSearchParameters());
        }
    }
}