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

namespace RealEstate.Web.Pages.Manage.Presence
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
        public PresenceInputViewModel NewPresence { get; set; }

        [ViewData]
        public string PageTitle { get; set; }

        [TempData]
        public string PresenceStatus { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                if (!User.IsInRole(nameof(Role.SuperAdmin)) && !User.IsInRole(nameof(Role.Admin)))
                    return Forbid();

                var model = await _employeeService.PresenceInputAsync(id).ConfigureAwait(false);
                if (model == null)
                    return RedirectToPage(typeof(IndexModel).Page());

                NewPresence = model;
                PageTitle = _localizer["EditPresence"];
            }
            else
            {
                NewPresence = new PresenceInputViewModel
                {
                    Date = DateTime.Now.GregorianToPersian(true),
                    Time = $"{DateTime.Now.Hour}:{DateTime.Now.Minute}"
                };
                PageTitle = _localizer["NewPresence"];
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var finalStatus = ModelState.IsValid
                ? (await _employeeService.PresenceAddOrUpdateAsync(NewPresence, !NewPresence.IsNew, true).ConfigureAwait(false)).Item1
                : StatusEnum.RetryAfterReview;

            PresenceStatus = finalStatus.GetDisplayName();
            if (finalStatus != StatusEnum.Success || !NewPresence.IsNew)
                return Page();

            ModelState.Clear();
            NewPresence = default;

            return RedirectToPage(typeof(AddModel).Page(), new
            {
                id = NewPresence?.Id
            });
        }
    }
}