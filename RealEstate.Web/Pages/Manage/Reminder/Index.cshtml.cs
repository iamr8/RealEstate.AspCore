using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RealEstate.Base;
using RealEstate.Base.Attributes;
using RealEstate.Resources;
using RealEstate.Services.Extensions;
using RealEstate.Services.ServiceLayer;
using RealEstate.Services.ViewModels.ModelBind;
using RealEstate.Services.ViewModels.Search;
using System.Threading.Tasks;

namespace RealEstate.Web.Pages.Manage.Reminder
{
    [NavBarHelper(typeof(IndexModel))]
    public class IndexModel : PageModel
    {
        private readonly IReminderService _reminderService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public IndexModel(
            IReminderService reminderService,
            IStringLocalizer<SharedResource> sharedLocalizer)
        {
            _reminderService = reminderService;
            _localizer = sharedLocalizer;
        }

        [BindProperty]
        public ReminderSearchViewModel SearchInput { get; set; }

        public PaginationViewModel<ReminderViewModel> List { get; set; }

        [ViewData]
        public string PageTitle => _localizer[SharedResource.Reminders];

        public async Task OnGetAsync(string pageNo)
        {
            SearchInput = new ReminderSearchViewModel
            {
                PageNo = pageNo.FixPageNumber(),
            };

            List = await _reminderService.ReminderListAsync(SearchInput).ConfigureAwait(false);
        }

        public IActionResult OnPost()
        {
            return RedirectToPage(typeof(IndexModel).Page(), SearchInput.GetSearchParameters());
        }
    }
}