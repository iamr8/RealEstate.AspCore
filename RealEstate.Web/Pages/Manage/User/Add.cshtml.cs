using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RealEstate.Base.Attributes;
using RealEstate.Base.Enums;
using RealEstate.Resources;
using RealEstate.Services.ServiceLayer;
using RealEstate.Services.ServiceLayer.Base;
using RealEstate.Services.ViewModels.Input;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Web.Pages.Manage.User
{
    [NavBarHelper(typeof(IndexModel))]
    public class AddModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IBaseService _baseService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public AddModel(
            IUserService userService,
            IBaseService baseService,
            IStringLocalizer<SharedResource> sharedLocalizer
            )
        {
            _userService = userService;
            _localizer = sharedLocalizer;
            _baseService = baseService;
        }

        [BindProperty]
        public UserInputViewModel NewUser { get; set; }

        [ViewData]
        public string PageTitle { get; set; }

        public StatusEnum Status { get; set; }

        public async Task<IActionResult> OnGetAsync(string id, string status)
        {
            UserInputViewModel model = null;
            if (!string.IsNullOrEmpty(id))
            {
                if (!User.IsInRole(nameof(Role.SuperAdmin)) && !User.IsInRole(nameof(Role.Admin)))
                    return Forbid();

                model = await _userService.FindInputAsync(id).ConfigureAwait(false);
            }
            else
            {
                if (User.IsInRole(nameof(Role.User)))
                {
                    var currentUser = _baseService.CurrentUser();
                    if (currentUser != null)
                        model = await _userService.FindInputAsync(currentUser.Id).ConfigureAwait(false);
                }
            }

            PageTitle = _localizer[(model == null ? "New" : "Edit") + GetType().Namespaces().Last()];
            NewUser = model;
            Status = !string.IsNullOrEmpty(status) && int.TryParse(status, out var statusInt)
                ? (StatusEnum)statusInt
                : StatusEnum.Ready;

            if (!string.IsNullOrEmpty(id) && model == null)
                return RedirectToPage(typeof(IndexModel).Page());

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var finalStatus = ModelState.IsValid
                ? (await _userService.AddOrUpdateAsync(NewUser, !NewUser.IsNew, true).ConfigureAwait(false)).Status
                : StatusEnum.RetryAfterReview;

            return RedirectToPage(finalStatus != StatusEnum.Success
                ? typeof(AddModel).Page()
                : typeof(IndexModel).Page(), new
                {
                    status = (int)finalStatus
                });
        }
    }
}