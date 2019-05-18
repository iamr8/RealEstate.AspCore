using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RealEstate.Base;
using RealEstate.Base.Attributes;
using RealEstate.Base.Enums;
using RealEstate.Resources;
using RealEstate.Services.ServiceLayer;
using RealEstate.Services.ViewModels.Input;
using System.Threading.Tasks;

namespace RealEstate.Web.Pages.Manage.Division
{
    [NavBarHelper(typeof(IndexModel))]
    public class AddModel : PageModel
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

        [ViewData]
        public string PageTitle { get; set; }

        [TempData]
        public string DivisionStatus { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                if (!User.IsInRole(nameof(Role.SuperAdmin)) && !User.IsInRole(nameof(Role.Admin)))
                    return Forbid();

                var model = await _divisionService.InputAsync(id).ConfigureAwait(false);
                if (model == null)
                    return RedirectToPage(typeof(IndexModel).Page());

                NewDivision = model;
                PageTitle = _localizer["EditDivision"];
            }
            else
            {
                PageTitle = _localizer["NewDivision"];
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var finalStatus = ModelState.IsValid
                ? (await _divisionService.AddOrUpdateAsync(NewDivision, !NewDivision.IsNew, true).ConfigureAwait(false)).Status
                : StatusEnum.RetryAfterReview;

            DivisionStatus = finalStatus.GetDisplayName();
            if (finalStatus != StatusEnum.Success || !NewDivision.IsNew)
                return Page();

            ModelState.Clear();
            NewDivision = default;

            return RedirectToPage(typeof(AddModel).Page(), new
            {
                id = NewDivision?.Id
            });
        }
    }
}