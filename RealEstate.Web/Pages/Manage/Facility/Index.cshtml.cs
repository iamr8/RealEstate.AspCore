using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    [Authorize(Roles = "Admin,SuperAdmin")]
    [NavBarHelper(typeof(IndexModel))]
    public class IndexModel : IndexPageModel
    {
        private readonly IFeatureService _featureService;

        public IndexModel(
            IFeatureService featureService,
            IStringLocalizer<SharedResource> sharedLocalizer)
        {
            _featureService = featureService;
            PageTitle = sharedLocalizer[SharedResource.Facilities];
        }

        [BindProperty]
        public FacilitySearchViewModel SearchInput { get; set; }

        public PaginationViewModel<FacilityViewModel> List { get; set; }

        public async Task OnGetAsync(string pageNo, string facilityName, bool deleted, string dateFrom, string dateTo, string creatorId, string status)
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

            Status = !string.IsNullOrEmpty(status)
                ? status
                : null;
            List = await _featureService.FacilityListAsync(SearchInput).ConfigureAwait(false);
        }

        public IActionResult OnPost()
        {
            return RedirectToPage(typeof(IndexModel).Page(), SearchInput.GetSearchParameters());
        }
    }
}