using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using RealEstate.Base;
using RealEstate.Base.Attributes;
using RealEstate.Resources;
using RealEstate.Services.Extensions;
using RealEstate.Services.ServiceLayer;
using RealEstate.Services.ViewModels.ModelBind;
using RealEstate.Services.ViewModels.Search;
using System.Threading.Tasks;

namespace RealEstate.Web.Pages.Manage.Employee
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    [NavBarHelper(typeof(IndexModel))]
    public class IndexModel : IndexPageModel
    {
        private readonly IEmployeeService _employeeService;

        public IndexModel(
            IEmployeeService employeeService,
            IStringLocalizer<SharedResource> sharedLocalizer)
        {
            _employeeService = employeeService;
            PageTitle = sharedLocalizer[SharedResource.Employees];
        }

        [BindProperty]
        public EmployeeSearchViewModel SearchInput { get; set; }

        public PaginationViewModel<EmployeeViewModel> List { get; set; }

        public string Status { get; set; }

        public async Task OnGetAsync(string pageNo, string employeeFirstName, string employeeLastName, string employeeMobile, string employeePhone,
            string employeeAddress, string employeeId, string userId, string divisionId, bool deleted, string status)
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

            Status = !string.IsNullOrEmpty(status)
                ? status
                : null;
            List = await _employeeService.ListAsync(SearchInput).ConfigureAwait(false);
        }

        public IActionResult OnPost()
        {
            return RedirectToPage(typeof(Employee.IndexModel).Page(), SearchInput.GetSearchParameters());
        }
    }
}