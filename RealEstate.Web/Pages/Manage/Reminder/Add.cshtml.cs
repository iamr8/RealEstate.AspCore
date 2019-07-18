using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using RealEstate.Base.Attributes;
using RealEstate.Resources;
using RealEstate.Services.Extensions;
using RealEstate.Services.ServiceLayer;
using RealEstate.Services.ViewModels.Input;
using System.Threading.Tasks;

namespace RealEstate.Web.Pages.Manage.Reminder
{
    [NavBarHelper(typeof(IndexModel))]
    public class AddModel : AddPageModel
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

        public async Task<IActionResult> OnGetAsync(string id, string status)
        {
            var result = await this.OnGetHandlerAsync(id, status,
                identifier => _reminderService.ReminderInputAsync(identifier),
                typeof(IndexModel).Page(),
                true);
            return result;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var result = await this.OnPostHandlerAsync(
                () => _reminderService.ReminderAddAsync(NewReminder, true),
                typeof(IndexModel).Page(),
                typeof(AddModel).Page());
            return result;
        }
    }
}