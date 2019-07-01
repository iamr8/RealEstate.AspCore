using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

namespace RealEstate.Web.Pages.Manage.Feature
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
            PageTitle = sharedLocalizer[SharedResource.Features];
        }

        [BindProperty]
        public FeatureSearchViewModel SearchInput { get; set; }

        public PaginationViewModel<FeatureViewModel> List { get; set; }

        public async Task OnGetAsync(string pageNo, string featureName, bool deleted, FeatureTypeEnum? type, string dateFrom, string dateTo, string creatorId, string status)
        {
            SearchInput = new FeatureSearchViewModel
            {
                PageNo = pageNo.FixPageNumber(),
                Name = featureName,
                IncludeDeletedItems = deleted,
                Type = type,
                CreatorId = creatorId,
                CreationDateFrom = dateFrom,
                CreationDateTo = dateTo
            };

            Status = !string.IsNullOrEmpty(status)
                ? status
                : null;
            List = await _featureService.FeatureListAsync(SearchInput).ConfigureAwait(false);
        }

        public IActionResult OnPost()
        {
            return RedirectToPage(typeof(IndexModel).Page(), SearchInput.GetSearchParameters());
        }
    }
}