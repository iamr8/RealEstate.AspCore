using Microsoft.AspNetCore.Authorization;
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

namespace RealEstate.Web.Pages.Manage.Feature
{
    [Authorize(Roles = "Admin,SuperAdmin")]
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
        public FeatureSearchViewModel SearchInput { get; set; }

        public PaginationViewModel<FeatureViewModel> List { get; set; }

        public int PageNo { get; set; }

        [ViewData]
        public string PageTitle { get; set; }

        public async Task OnGetAsync(string pageNo, string featureName)
        {
            SearchInput = new FeatureSearchViewModel
            {
                Name = featureName
            };

            PageTitle = _localizer["Features"];
            PageNo = pageNo.FixPageNumber();
            List = await _featureService.FeatureListAsync(PageNo, featureName, null).ConfigureAwait(false);
        }

        public IActionResult OnPost()
        {
            return RedirectToPage(typeof(IndexModel).Page(),
                new
                {
                    featureName = SearchInput.Name,
                });
        }
    }
}