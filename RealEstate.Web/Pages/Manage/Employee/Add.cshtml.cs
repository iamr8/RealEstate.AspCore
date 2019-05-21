using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RealEstate.Base.Attributes;
using RealEstate.Base.Enums;
using RealEstate.Resources;
using RealEstate.Services.Extensions;
using RealEstate.Services.ServiceLayer;
using RealEstate.Services.ViewModels.Input;
using System.Threading.Tasks;

namespace RealEstate.Web.Pages.Manage.Employee
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    [NavBarHelper(typeof(IndexModel))]
    public class AddModel : PageModel
    {
        private readonly IEmployeeService _employeeService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public AddModel(
            IEmployeeService employeeService,
            IStringLocalizer<SharedResource> sharedLocalizer
            )
        {
            _employeeService = employeeService;
            _localizer = sharedLocalizer;
        }

        [BindProperty]
        public EmployeeInputViewModel NewEmployee { get; set; }

        [ViewData]
        public string PageTitle { get; set; }

        public string Status { get; set; }

        public async Task<IActionResult> OnGetAsync(string id, string status)
        {
            if (!string.IsNullOrEmpty(id))
            {
                if (!User.IsInRole(nameof(Role.SuperAdmin)))
                    return Forbid();

                var model = await _employeeService.EmployeeInputAsync(id).ConfigureAwait(false);
                if (model == null)
                    return RedirectToPage(typeof(IndexModel).Page());

                NewEmployee = model;
                PageTitle = _localizer[SharedResource.EditEmployee];
            }
            else
            {
                PageTitle = _localizer[SharedResource.NewEmployee];
            }
            Status = !string.IsNullOrEmpty(status)
                ? status
                : null;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var (status, message) = await ModelState.IsValidAsync(
                () => _employeeService.AddOrUpdateAsync(NewEmployee, !NewEmployee.IsNew, true)
            ).ConfigureAwait(false);

            return RedirectToPage(status != StatusEnum.Success
                ? typeof(AddModel).Page()
                : typeof(IndexModel).Page(), new
                {
                    status = message,
                    id = status != StatusEnum.Success ? NewEmployee?.Id : null
                });
        }
    }
}