using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Resources;
using RealEstate.Services;
using RealEstate.Services.ViewModels.Input;
using System.Threading.Tasks;
using RealEstate.Services.Extensions;
using RealEstate.Services.ServiceLayer;

namespace RealEstate.Web.Pages.Manage.Reminder
{
    public class AddModel : PageModel
    {
        private readonly IReminderService _reminderService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public AddModel(
            IReminderService reminderService,
            IStringLocalizer<SharedResource> sharedLocalizer
            )
        {
            _reminderService = reminderService;
            _localizer = sharedLocalizer;
        }

        [BindProperty]
        public ReminderInputViewModel NewReminder { get; set; }

        [ViewData]
        public string PageTitle { get; set; }

        [TempData]
        public string ReminderStatus { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                if (!User.IsInRole(nameof(Role.SuperAdmin)) && !User.IsInRole(nameof(Role.Admin)))
                    return Forbid();

                var model = await _reminderService.ReminderInputAsync(id).ConfigureAwait(false);
                if (model == null)
                    return RedirectToPage(typeof(IndexModel).Page());

                NewReminder = model;
                PageTitle = _localizer["EditReminder"];
            }
            else
            {
                NewReminder = new ReminderInputViewModel
                {
                    Date = DateTime.Now.GregorianToPersian(true)
                };
                PageTitle = _localizer["NewReminder"];
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var finalStatus = ModelState.IsValid
                ? (await _reminderService.ReminderAddOrUpdateAsync(NewReminder, !NewReminder.IsNew, true).ConfigureAwait(false)).Status
                : StatusEnum.RetryAfterReview;

            ReminderStatus = finalStatus.GetDisplayName();
            if (finalStatus != StatusEnum.Success || !NewReminder.IsNew)
                return Page();

            ModelState.Clear();
            NewReminder = default;

            return RedirectToPage(typeof(AddModel).Page(), new
            {
                id = NewReminder?.Id
            });
        }
    }
}