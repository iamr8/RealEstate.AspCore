using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using RealEstate.Base;
using RealEstate.Base.Attributes;
using RealEstate.Resources;
using RealEstate.Services.Extensions;
using RealEstate.Services.ServiceLayer;
using RealEstate.Services.ViewComponents;
using RealEstate.Services.ViewModels.ModelBind;
using RealEstate.Services.ViewModels.Search;

namespace RealEstate.Web.Pages.Manage.Reminder
{
    [NavBarHelper(typeof(IndexModel))]
    public class IndexModel : IndexPageModel
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

        public async Task OnGetAsync(string pageNo, string status, bool deleted, string dateFrom, string dateTo, string creatorId, decimal? prc, string chkb, string chkn, string subj, string from, string to)
        {
            SearchInput = new ReminderSearchViewModel
            {
                PageNo = pageNo.FixPageNumber(),
                IncludeDeletedItems = deleted,
                CreatorId = creatorId,
                CreationDateFrom = dateFrom,
                CreationDateTo = dateTo,
                Price = prc,
                CheckBank = chkb,
                CheckNumber = chkn,
                Subject = subj,
                FromDate = from,
                ToDate = to
            };

            Status = !string.IsNullOrEmpty(status)
                ? status
                : null;
            List = await _reminderService.ReminderListAsync(SearchInput, false);
        }

        public IActionResult OnPost()
        {
            return RedirectToPage(typeof(IndexModel).Page(), SearchInput.RouteDictionary());
        }

        public async Task<IActionResult> OnGetPageAsync(ReminderSearchViewModel models)
        {
            var list = await _reminderService.ReminderListAsync(models);
            return ViewComponent(typeof(ReminderPageViewComponent), new
            {
                models = list.Items
            });
        }
    }
}