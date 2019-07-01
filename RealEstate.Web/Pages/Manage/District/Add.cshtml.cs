using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using RealEstate.Base.Attributes;
using RealEstate.Resources;
using RealEstate.Services.Extensions;
using RealEstate.Services.ServiceLayer;
using RealEstate.Services.ViewModels.Input;
using System.Threading.Tasks;

namespace RealEstate.Web.Pages.Manage.District
{
    [NavBarHelper(typeof(IndexModel))]
    public class AddModel : AddPageModel
    {
        private readonly ILocationService _locationService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public AddModel(
            ILocationService locationService,
            IStringLocalizer<SharedResource> sharedLocalizer
            )
        {
            _locationService = locationService;
            _localizer = sharedLocalizer;
        }

        [BindProperty]
        public DistrictInputViewModel NewDistrict { get; set; }

        public async Task<IActionResult> OnGetAsync(string id, string status)
        {
            var result = await this.OnGetHandlerAsync(id, status,
                identifier => _locationService.DistrictInputAsync(id),
                typeof(IndexModel).Page(),
                true);
            return result;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var result = await this.OnPostHandlerAsync(
                () => _locationService.DistrictAddOrUpdateAsync(NewDistrict, !NewDistrict.IsNew, true),
                typeof(IndexModel).Page(),
                typeof(AddModel).Page());
            return result;
        }
    }
}