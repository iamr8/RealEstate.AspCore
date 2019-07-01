using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using RealEstate.Base.Attributes;
using RealEstate.Resources;
using RealEstate.Services.Extensions;
using RealEstate.Services.ServiceLayer;
using RealEstate.Services.ViewModels.Input;
using System.Threading.Tasks;

namespace RealEstate.Web.Pages.Manage.Category
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
        public CategoryInputViewModel NewCategory { get; set; }

        public async Task<IActionResult> OnGetAsync(string id, string status)
        {
            var result = await this.OnGetHandlerAsync(id, status,
                identifier => _featureService.CategoryInputAsync(id),
                typeof(IndexModel).Page(),
                true);
            return result;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var result = await this.OnPostHandlerAsync(
                () => _featureService.CategoryAddOrUpdateAsync(NewCategory, !NewCategory.IsNew, true),
                typeof(IndexModel).Page(),
                typeof(AddModel).Page());
            return result;
        }
    }
}