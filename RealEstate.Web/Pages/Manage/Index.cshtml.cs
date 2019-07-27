using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RealEstate.Base;
using RealEstate.Base.Attributes;
using RealEstate.Resources;
using RealEstate.Services.ServiceLayer;
using RealEstate.Services.ViewModels;
using RealEstate.Services.ViewModels.Search;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealEstate.Web.Pages.Manage
{
    [NavBarHelper(typeof(IndexModel))]
    public class IndexModel : PageModel
    {
        private readonly IGlobalService _globalService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public IndexModel(
            IGlobalService globalService,
            IStringLocalizer<SharedResource> localizer
            )
        {
            _globalService = globalService;
            _localizer = localizer;
        }

        [BindProperty]
        public StatisticsSearchViewModel SearchInput { get; set; }

        public string PageTitle => _localizer[SharedResource.Statistics];

        public List<StatisticsViewModel> Statistics { get; set; }
        public string Status { get; set; }

        public async Task<IActionResult> OnGetAsync(string status, string dateFrom, string dateTo, string creatorId)
        {
            SearchInput = new StatisticsSearchViewModel
            {
                CreatorId = creatorId,
                CreationDateFrom = dateFrom,
                CreationDateTo = dateTo
            };

            Status = !string.IsNullOrEmpty(status)
                ? status
                : null;
            Statistics = await _globalService.StatisticsAsync(SearchInput).ConfigureAwait(false);
            return Page();
        }

        public IActionResult OnPost()
        {
            return RedirectToPage(typeof(IndexModel).Page(), SearchInput.RouteDictionary());
        }
    }
}