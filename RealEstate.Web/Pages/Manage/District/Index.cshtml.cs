using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RealEstate.Base;
using RealEstate.Extensions;
using RealEstate.Resources;
using RealEstate.Services;
using RealEstate.ViewModels;
using RealEstate.ViewModels.Search;
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

        public int PageNo { get; set; }

        [ViewData]
        public string PageTitle { get; set; }

        public async Task OnGetAsync(string pageNo, string districtName)
        {
            SearchInput = new DistrictSearchViewModel
            {
                Name = districtName,
            };

            PageTitle = _localizer["Districts"];
            PageNo = pageNo.FixPageNumber();
            List = await _locationService.DistrictListAsync(PageNo, districtName).ConfigureAwait(false);
        }

        public IActionResult OnPost()
        {
            return RedirectToPage(typeof(IndexModel).Page(), SearchInput.GetSearchParameters());
        }
    }
}