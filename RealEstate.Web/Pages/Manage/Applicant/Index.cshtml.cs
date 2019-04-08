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

namespace RealEstate.Web.Pages.Manage.Applicant
{
    public class IndexModel : PageModel
    {
        private readonly IContactService _contactService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public IndexModel(
            IContactService contactService,
            IStringLocalizer<SharedResource> sharedLocalizer)
        {
            _contactService = contactService;
            _localizer = sharedLocalizer;
        }

        [BindProperty]
        public ApplicantSearchViewModel SearchInput { get; set; }

        public PaginationViewModel<ApplicantViewModel> List { get; set; }

        [ViewData]
        public string PageTitle => _localizer["Applicants"];

        public async Task OnGetAsync(string pageNo)
        {
            SearchInput = new ApplicantSearchViewModel
            {
                PageNo = pageNo.FixPageNumber()
            };

            List = await _contactService.ApplicantListAsync(SearchInput).ConfigureAwait(false);
        }

        public IActionResult OnPost()
        {
            return RedirectToPage(typeof(IndexModel).Page(), SearchInput.GetSearchParameters());
        }
    }
}