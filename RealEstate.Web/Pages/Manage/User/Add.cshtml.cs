using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RealEstate.Base.Enums;
using RealEstate.Extensions;
using RealEstate.Resources;
using RealEstate.Services;
using RealEstate.ViewModels.Input;
using System.Threading.Tasks;
using RealEstate.Base;

namespace RealEstate.Web.Pages.Manage.User
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class AddModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public AddModel(
            IUserService userService,
            IStringLocalizer<SharedResource> sharedLocalizer
            )
        {
            _userService = userService;
            _localizer = sharedLocalizer;
        }

        [BindProperty]
        public UserInputViewModel NewUser { get; set; }

        [ViewData]
        public string PageTitle { get; set; }

        [TempData]
        public string UserStatus { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                if (!User.IsInRole(nameof(Role.SuperAdmin)))
                    return Forbid();

                var model = await _userService.FindInputAsync(id).ConfigureAwait(false);
                if (model == null)
                    return RedirectToPage(typeof(IndexModel).Page());

                NewUser = model;
                PageTitle = _localizer["EditUser"];
            }
            else
            {
                PageTitle = _localizer["NewUser"];
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var finalStatus = ModelState.IsValid
                ? (await _userService.CategoryAddOrUpdateAsync(NewUser, !NewUser.IsNew, true).ConfigureAwait(false)).Item1
                : StatusEnum.RetryAfterReview;

            UserStatus = finalStatus.GetDisplayName();
            if (finalStatus != StatusEnum.Success || !NewUser.IsNew)
                return Page();

            ModelState.Clear();
            NewUser = default;

            return RedirectToPage(typeof(AddModel).Page(), new
            {
                id = NewUser?.Id
            });
        }
    }
}