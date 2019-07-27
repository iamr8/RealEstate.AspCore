using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RealEstate.Base;
using RealEstate.Base.Attributes;
using RealEstate.Resources;
using RealEstate.Services.Extensions;
using RealEstate.Services.ServiceLayer;
using RealEstate.Services.ViewComponents;
using RealEstate.Services.ViewModels;
using RealEstate.Services.ViewModels.ModelBind;
using RealEstate.Services.ViewModels.Search;

namespace RealEstate.Web.Pages.Manage.Applicant
{
    [NavBarHelper(typeof(IndexModel))]
    public class IndexModel : PageModel
    {
        private readonly ICustomerService _customerService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public IndexModel(
            ICustomerService customerService,
            IStringLocalizer<SharedResource> sharedLocalizer)
        {
            _customerService = customerService;
            _localizer = sharedLocalizer;
        }

        [BindProperty]
        public ApplicantSearchViewModel SearchInput { get; set; }

        [BindProperty]
        public TransApplicantViewModel TransInput { get; set; }

        public PaginationViewModel<ApplicantViewModel> List { get; set; }

        public string Status { get; set; }

        public string PageTitle => _localizer[SharedResource.Applicants];

        public async Task OnGetAsync(string pageNo, string status)
        {
            SearchInput = new ApplicantSearchViewModel
            {
                PageNo = pageNo.FixPageNumber()
            };

            Status = !string.IsNullOrEmpty(status) ? status : null;
            List = await _customerService.ApplicantListAsync(SearchInput, false);
        }

        public IActionResult OnPost()
        {
            return RedirectToPage(typeof(IndexModel).Page(), SearchInput.RouteDictionary());
        }

        public async Task<IActionResult> OnPostTransAsync()
        {
            var (status, message) = await ModelState.IsValidAsync(
                () => _customerService.ShiftApplicantAsync(TransInput),
                nameof(TransInput));
            return RedirectToPage(typeof(IndexModel).Page(), SearchInput.RouteDictionary());
        }

        public async Task<IActionResult> OnGetPageAsync(ApplicantSearchViewModel models)
        {
            var list = await _customerService.ApplicantListAsync(models);
            return ViewComponent(typeof(ApplicantPageViewComponent), new
            {
                models = list.Items
            });
        }
    }
}