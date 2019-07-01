using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using RealEstate.Base.Attributes;
using RealEstate.Resources;
using RealEstate.Services.Extensions;
using RealEstate.Services.ServiceLayer;
using RealEstate.Services.ViewModels.Input;
using System.Threading.Tasks;

namespace RealEstate.Web.Pages.Manage.Facility
{
    [NavBarHelper(typeof(IndexModel))]
    public class AddModel : AddPageModel
    {
        private readonly IFeatureService _featureService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public AddModel(
            IFeatureService featureService,
            IStringLocalizer<SharedResource> sharedLocalizer
            )
        {
            _featureService = featureService;
            _localizer = sharedLocalizer;
        }

        [BindProperty]
        public FacilityInputViewModel NewFacility { get; set; }

        public async Task<IActionResult> OnGetAsync(string id, string status)
        {
            var result = await this.OnGetHandlerAsync(id, status,
                identifier => _featureService.FacilityInputAsync(id),
                typeof(IndexModel).Page(),
                true);
            return result;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var result = await this.OnPostHandlerAsync(
                () => _featureService.FacilityAddOrUpdateAsync(NewFacility, !NewFacility.IsNew, true),
                typeof(IndexModel).Page(),
                typeof(AddModel).Page());
            return result;
        }
    }
}