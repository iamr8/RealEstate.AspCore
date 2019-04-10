using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RealEstate.Base;
using RealEstate.Extensions;
using RealEstate.Resources;
using RealEstate.Services;
using RealEstate.Services.ViewModels;
using RealEstate.Services.ViewModels.Search;
using System.Threading.Tasks;

namespace RealEstate.Web.Pages.Manage.Facility
{
    public class IndexModel : PageModel
    {
        private readonly IFeatureService _featureService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public IndexModel(
            IFeatureService featureService,
            IStringLocalizer<SharedResource> sharedLocalizer)
        {
            _featureService = featureService;
            _localizer = sharedLocalizer;
        }

        [BindProperty]
        public FacilitySearchViewModel SearchInput { get; set; }

        public PaginationViewModel<FacilityViewModel> List { get; set; }

        [ViewData]
        public string PageTitle => _localizer["Facilities"];

        public async Task OnGetAsync(string pageNo, string facilityName)
        {
            SearchInput = new FacilitySearchViewModel
            {
                PageNo = pageNo.FixPageNumber(),
                Name = facilityName
            };

            List = await _featureService.FacilityListAsync(SearchInput).ConfigureAwait(false);
        }

        public IActionResult OnPost()
        {
            return RedirectToPage(typeof(Facility.IndexModel).Page(), SearchInput.GetSearchParameters());
        }
    }
}