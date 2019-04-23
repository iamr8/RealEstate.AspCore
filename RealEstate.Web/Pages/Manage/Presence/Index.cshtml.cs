using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RealEstate.Base;
using RealEstate.Resources;
using RealEstate.Services;
using RealEstate.Services.Extensions;
using RealEstate.Services.ViewModels;
using RealEstate.Services.ViewModels.Search;
using System.Threading.Tasks;

namespace RealEstate.Web.Pages.Manage.Presence
{
    public class IndexModel : PageModel
    {
        private readonly IEmployeeService _employeeService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public IndexModel(
            IEmployeeService employeeService,
            IStringLocalizer<SharedResource> sharedLocalizer)
        {
            _employeeService = employeeService;
            _localizer = sharedLocalizer;
        }

        [BindProperty]
        public PresenceSearchViewModel SearchInput { get; set; }

        public PaginationViewModel<PresenceViewModel> List { get; set; }

        [ViewData]
        public string PageTitle => _localizer["Presences"];

        public async Task OnGetAsync(string pageNo)
        {
            SearchInput = new PresenceSearchViewModel
            {
                PageNo = pageNo.FixPageNumber(),
            };

            List = await _employeeService.PresenceListAsync(SearchInput).ConfigureAwait(false);
        }

        public IActionResult OnPost()
        {
            return RedirectToPage(typeof(IndexModel).Page(), SearchInput.GetSearchParameters());
        }
    }
}