using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RealEstate.Resources;
using RealEstate.Services.ServiceLayer;

namespace RealEstate.Web.Pages.Manage.Settings
{
    public class IndexModel : PageModel
    {
        private readonly IGlobalService _globalService;

        public IndexModel(
            IGlobalService globalService,
            IStringLocalizer<SharedResource> stringLocalizer)
        {
            _globalService = globalService;
            PageTitle = stringLocalizer[SharedResource.Settings];
        }

        [ViewData]
        public string PageTitle { get; }

        public IActionResult OnGet()
        {
            return Page();
        }
    }
}