using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using RealEstate.Base;
using RealEstate.Base.Attributes;
using RealEstate.Resources;
using RealEstate.Services.Extensions;
using RealEstate.Services.ServiceLayer;
using RealEstate.Services.ViewComponents;
using RealEstate.Services.ViewModels.ModelBind;
using RealEstate.Services.ViewModels.Search;

namespace RealEstate.Web.Pages.Manage.District
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    [NavBarHelper(typeof(IndexModel))]
    public class IndexModel : IndexPageModel
    {
        private readonly ILocationService _locationService;

        public IndexModel(
            ILocationService locationService,
            IStringLocalizer<SharedResource> sharedLocalizer)
        {
            _locationService = locationService;
            PageTitle = sharedLocalizer[SharedResource.Districts];
        }

        [BindProperty]
        public DistrictSearchViewModel SearchInput { get; set; }

        public PaginationViewModel<DistrictViewModel> List { get; set; }

        public async Task OnGetAsync(string pageNo, string districtName, bool deleted, string dateFrom, string dateTo, string creatorId, string status)
        {
            SearchInput = new DistrictSearchViewModel
            {
                PageNo = pageNo.FixPageNumber(),
                Name = districtName,
                IncludeDeletedItems = deleted,
                CreatorId = creatorId,
                CreationDateFrom = dateFrom,
                CreationDateTo = dateTo
            };

            Status = !string.IsNullOrEmpty(status)
                ? status
                : null;
            List = await _locationService.DistrictListAsync(SearchInput, false);
        }

        public IActionResult OnPost()
        {
            return RedirectToPage(typeof(IndexModel).Page(), SearchInput.RouteDictionary());
        }

        public async Task<IActionResult> OnGetPageAsync(DistrictSearchViewModel models)
        {
            var list = await _locationService.DistrictListAsync(models);
            return ViewComponent(typeof(DistrictPageViewComponent), new
            {
                models = list.Items
            });
        }
    }
}