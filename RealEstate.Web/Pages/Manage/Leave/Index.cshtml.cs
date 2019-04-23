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

namespace RealEstate.Web.Pages.Manage.Leave
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
        public LeaveSearchViewModel SearchInput { get; set; }

        public PaginationViewModel<LeaveViewModel> List { get; set; }

        [ViewData]
        public string PageTitle => _localizer["Leaves"];

        public async Task OnGetAsync(string pageNo)
        {
            SearchInput = new LeaveSearchViewModel
            {
                PageNo = pageNo.FixPageNumber(),
            };

            List = await _employeeService.LeaveListAsync(SearchInput).ConfigureAwait(false);
        }

        public IActionResult OnPost()
        {
            return RedirectToPage(typeof(IndexModel).Page(), SearchInput.GetSearchParameters());
        }
    }
}