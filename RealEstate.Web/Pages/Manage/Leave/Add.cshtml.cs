using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Resources;
using RealEstate.Services;
using RealEstate.Services.Extensions;
using RealEstate.Services.ViewModels.Input;
using System;
using System.Threading.Tasks;

namespace RealEstate.Web.Pages.Manage.Leave
{
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
        public LeaveInputViewModel NewLeave { get; set; }

        [ViewData]
        public string PageTitle { get; set; }

        [TempData]
        public string LeaveStatus { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                if (!User.IsInRole(nameof(Role.SuperAdmin)) && !User.IsInRole(nameof(Role.Admin)))
                    return Forbid();

                var model = await _employeeService.LeaveInputAsync(id).ConfigureAwait(false);
                if (model == null)
                    return RedirectToPage(typeof(IndexModel).Page());

                NewLeave = model;
                PageTitle = _localizer["EditLeave"];
            }
            else
            {
                NewLeave = new LeaveInputViewModel
                {
                    FromDate = DateTime.Now.GregorianToPersian(true),
                    FromHour = DateTime.Now.Hour,
                    ToDate = DateTime.Now.GregorianToPersian(true),
                    ToHour = DateTime.Now.Hour
                };
                PageTitle = _localizer["NewLeave"];
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var finalStatus = ModelState.IsValid
                ? (await _employeeService.LeaveAddOrUpdateAsync(NewLeave, !NewLeave.IsNew, true).ConfigureAwait(false)).Status
                : StatusEnum.RetryAfterReview;

            LeaveStatus = finalStatus.GetDisplayName();
            if (finalStatus != StatusEnum.Success || !NewLeave.IsNew)
                return Page();

            ModelState.Clear();
            NewLeave = default;

            return RedirectToPage(typeof(AddModel).Page(), new
            {
                id = NewLeave?.Id
            });
        }
    }
}