using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RealEstate.Resources;
using RealEstate.Services;
using RealEstate.Services.ViewModels;
using System.Threading.Tasks;
using RealEstate.Services.ServiceLayer;

namespace RealEstate.Web.Pages.Manage
{
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

        public string PageTitle => _localizer["Properties"];

        public StatisticsViewModel Statistics { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Statistics = await _globalService.StatisticsAsync().ConfigureAwait(false);

            return Page();
        }
    }
}