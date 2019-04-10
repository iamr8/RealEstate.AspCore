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

namespace RealEstate.Web.Pages.Manage.District
{
    public class IndexModel : PageModel
    {
        private readonly ILocationService _locationService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public IndexModel(
            ILocationService locationService,
            IStringLocalizer<SharedResource> sharedLocalizer)
        {
            _locationService = locationService;
            _localizer = sharedLocalizer;
        }

        [BindProperty]
        public DistrictSearchViewModel SearchInput { get; set; }

        public PaginationViewModel<DistrictViewModel> List { get; set; }

        [ViewData]
        public string PageTitle => _localizer["Districts"];

        public async Task OnGetAsync(string pageNo, string districtName)
        {
            SearchInput = new DistrictSearchViewModel
            {
                PageNo = pageNo.FixPageNumber(),
                Name = districtName,
            };

            List = await _locationService.DistrictListAsync(SearchInput).ConfigureAwait(false);
        }

        public IActionResult OnPost()
        {
            return RedirectToPage(typeof(IndexModel).Page(), SearchInput.GetSearchParameters());
        }
    }
}