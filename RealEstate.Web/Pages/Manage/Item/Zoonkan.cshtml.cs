using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RealEstate.Base.Attributes;
using RealEstate.Resources;
using RealEstate.Services.ServiceLayer;
using RealEstate.Services.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealEstate.Web.Pages.Manage.Item
{
    [NavBarHelper(typeof(ZoonkanModel))]
    public class ZoonkanModel : PageModel
    {
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly IItemService _itemService;

        public ZoonkanModel(
            IItemService itemService,
            IStringLocalizer<SharedResource> localizer
            )
        {
            _itemService = itemService;
            _localizer = localizer;
        }

        public string PageTitle => _localizer[SharedResource.Zoonkan];
        public List<ZoonkanViewModel> Zoonkans { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Zoonkans = await _itemService.ZoonkansAsync().ConfigureAwait(false);
            return Page();
        }
    }
}