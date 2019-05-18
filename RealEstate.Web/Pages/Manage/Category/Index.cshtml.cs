using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Resources;
using RealEstate.Services;
using RealEstate.Services.Extensions;
using RealEstate.Services.ViewModels;
using RealEstate.Services.ViewModels.Search;
using System.Threading.Tasks;
using RealEstate.Services.ServiceLayer;
using RealEstate.Services.ServiceLayer.Base;
using RealEstate.Services.ViewModels.ModelBind;

namespace RealEstate.Web.Pages.Manage.Category
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class IndexModel : PageModel
    {
        private readonly IFeatureService _featureService;
        private readonly IBaseService _baseService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public IndexModel(
            IFeatureService featureService,
            IBaseService baseService,
            IStringLocalizer<SharedResource> sharedLocalizer)
        {
            _featureService = featureService;
            _baseService = baseService;
            _localizer = sharedLocalizer;
        }

        [BindProperty]
        public CategorySearchViewModel SearchInput { get; set; }

        public PaginationViewModel<CategoryViewModel> List { get; set; }

        [ViewData]
        public string PageTitle => _localizer["Categories"];

        public async Task OnGetAsync(string pageNo, string categoryId, string categoryName, CategoryTypeEnum? type, bool deleted, string dateFrom, string dateTo, string creatorId)
        {
            SearchInput = new CategorySearchViewModel
            {
                PageNo = pageNo.FixPageNumber(),
                Name = categoryName,
                Id = categoryId,
                Type = type,
                IncludeDeletedItems = deleted,
                CreationDateFrom = dateFrom,
                CreationDateTo = dateTo,
                CreatorId = creatorId
            };

            List = await _featureService.CategoryListAsync(SearchInput).ConfigureAwait(false);
        }

        public IActionResult OnPost()
        {
            return RedirectToPage(typeof(IndexModel).Page(), SearchInput.GetSearchParameters());
        }
    }
}