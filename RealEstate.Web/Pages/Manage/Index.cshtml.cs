using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RealEstate.Base.Attributes;
using RealEstate.Resources;
using RealEstate.Services.ServiceLayer;
using RealEstate.Services.ViewModels;
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

        public string PageTitle => _localizer[SharedResource.Properties];

        public StatisticsViewModel Statistics { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Statistics = await _globalService.StatisticsAsync().ConfigureAwait(false);
            return Page();
        }
    }
}