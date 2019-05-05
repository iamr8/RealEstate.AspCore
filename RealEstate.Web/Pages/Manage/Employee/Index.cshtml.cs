using Microsoft.AspNetCore.Authorization;
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

namespace RealEstate.Web.Pages.Manage.Employee
{
    [Authorize(Roles = "Admin,SuperAdmin")]
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
        public EmployeeSearchViewModel SearchInput { get; set; }

        public PaginationViewModel<EmployeeViewModel> List { get; set; }

        [ViewData]
        public string PageTitle => _localizer["Employees"];

        public async Task OnGetAsync(string pageNo, string employeeFirstName, string employeeLastName, string employeeMobile, string employeePhone, string employeeAddress, string employeeId, string userId, string divisionId, bool deleted)
        {
            SearchInput = new EmployeeSearchViewModel
            {
                PageNo = pageNo.FixPageNumber(),
                Id = employeeId,
                Address = employeeAddress,
                Mobile = employeeMobile,
                Phone = employeePhone,
                FirstName = employeeFirstName,
                LastName = employeeLastName,
                UserId = userId,
                DivisionId = divisionId,
                IncludeDeletedItems = deleted
            };

            List = await _employeeService.ListAsync(SearchInput).ConfigureAwait(false);
        }

        public IActionResult OnPost()
        {
            return RedirectToPage(typeof(Employee.IndexModel).Page(), SearchInput.GetSearchParameters());
        }
    }
}