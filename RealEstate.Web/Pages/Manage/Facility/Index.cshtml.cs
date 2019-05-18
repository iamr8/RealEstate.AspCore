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

namespace RealEstate.Web.Pages.Manage.Facility
{
    [NavBarHelper(typeof(IndexModel))]
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
        public string PageTitle => _localizer[SharedResource.Facilities];

        public async Task OnGetAsync(string pageNo, string facilityName, bool deleted, string dateFrom, string dateTo, string creatorId)
        {
            SearchInput = new FacilitySearchViewModel
            {
                PageNo = pageNo.FixPageNumber(),
                Name = facilityName,
                IncludeDeletedItems = deleted,
                CreatorId = creatorId,
                CreationDateFrom = dateFrom,
                CreationDateTo = dateTo
            };

            List = await _featureService.FacilityListAsync(SearchInput).ConfigureAwait(false);
        }

        public IActionResult OnPost()
        {
            return RedirectToPage(typeof(Facility.IndexModel).Page(), SearchInput.GetSearchParameters());
        }
    }
}