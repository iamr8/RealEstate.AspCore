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
using RealEstate.Services.ViewModels.ModelBind;

namespace RealEstate.Web.Pages.Manage.Applicant
{
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

        public PaginationViewModel<ApplicantViewModel> List { get; set; }

        public string Status { get; set; }

        public string PageTitle => _localizer["Applicants"];

        public async Task OnGetAsync(string pageNo, string status)
        {
            SearchInput = new ApplicantSearchViewModel
            {
                PageNo = pageNo.FixPageNumber()
            };

            Status = int.TryParse(status, out var statusCode) ? ((StatusEnum)statusCode).GetDisplayName() : null;
            List = await _customerService.ApplicantListAsync(SearchInput).ConfigureAwait(false);
        }

        public IActionResult OnPost()
        {
            return RedirectToPage(typeof(IndexModel).Page(), SearchInput.GetSearchParameters());
        }
    }
}