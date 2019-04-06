using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Extensions;
using RealEstate.Resources;
using RealEstate.Services;
using RealEstate.Services.Base;
using RealEstate.ViewModels;
using RealEstate.ViewModels.Search;
using System.Threading.Tasks;

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

        public int PageNo { get; set; }

        [ViewData]
        public string PageTitle { get; set; }

        public async Task OnGetAsync(string pageNo, string categoryId, string categoryName, CategoryTypeEnum? type)
        {
            SearchInput = new CategorySearchViewModel
            {
                Name = categoryName,
                Id = categoryId,
                Type = type
            };

            PageTitle = _localizer["Categories"];
            PageNo = pageNo.FixPageNumber();
            List = await _featureService.CategoryListAsync(PageNo, categoryName, categoryId, type).ConfigureAwait(false);
        }

        public IActionResult OnPost()
        {
            return RedirectToPage(typeof(IndexModel).Page(),
                new
                {
                    categoryName = SearchInput.Name,
                    categoryId = SearchInput.Id,
                    type = SearchInput.Type
                });
        }
    }
}