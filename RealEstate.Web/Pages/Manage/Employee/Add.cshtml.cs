using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Resources;
using RealEstate.Services;
using RealEstate.Services.ViewModels.Input;
using System.Threading.Tasks;

namespace RealEstate.Web.Pages.Manage.Employee
{
    [Authorize(Roles = "Admin,SuperAdmin")]
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

        [TempData]
        public string EmployeeStatus { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                if (!User.IsInRole(nameof(Role.SuperAdmin)))
                    return Forbid();

                var model = await _employeeService.FindInputAsync(id).ConfigureAwait(false);
                if (model == null)
                    return RedirectToPage(typeof(IndexModel).Page());

                NewEmployee = model;
                PageTitle = _localizer["EditEmployee"];
            }
            else
            {
                PageTitle = _localizer["NewEmployee"];
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var finalStatus = ModelState.IsValid
                ? (await _employeeService.AddOrUpdateAsync(NewEmployee, !NewEmployee.IsNew, true).ConfigureAwait(false)).Item1
                : StatusEnum.RetryAfterReview;

            EmployeeStatus = finalStatus.GetDisplayName();
            if (finalStatus != StatusEnum.Success || !NewEmployee.IsNew)
                return Page();

            ModelState.Clear();
            NewEmployee = default;

            return RedirectToPage(typeof(AddModel).Page(), new
            {
                id = NewEmployee?.Id
            });
        }
    }
}