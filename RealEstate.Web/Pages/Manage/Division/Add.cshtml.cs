using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using RealEstate.Base.Attributes;
using RealEstate.Resources;
using RealEstate.Services.Extensions;
using RealEstate.Services.ServiceLayer;
using RealEstate.Services.ViewModels.Input;
using System.Threading.Tasks;

namespace RealEstate.Web.Pages.Manage.Division
{
    [NavBarHelper(typeof(IndexModel))]
    public class AddModel : AddPageModel
    {
        private readonly IDivisionService _divisionService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public AddModel(
            IDivisionService divisionService,
            IStringLocalizer<SharedResource> sharedLocalizer
            )
        {
            _divisionService = divisionService;
            _localizer = sharedLocalizer;
        }

        [BindProperty]
        public DivisionInputViewModel NewDivision { get; set; }

        public async Task<IActionResult> OnGetAsync(string id, string status)
        {
            var result = await this.OnGetHandlerAsync(id, status,
                identifier => _divisionService.InputAsync(id),
                typeof(IndexModel).Page(),
                true);
            return result;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var result = await this.OnPostHandlerAsync(
                () => _divisionService.AddOrUpdateAsync(NewDivision, !NewDivision.IsNew, true),
                typeof(IndexModel).Page(),
                typeof(AddModel).Page());
            return result;
        }
    }
}