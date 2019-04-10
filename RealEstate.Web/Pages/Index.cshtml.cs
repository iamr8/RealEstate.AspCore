using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Services;
using RealEstate.Services.Base;
using RealEstate.Services.ViewModels.Input;
using System.Threading.Tasks;

namespace RealEstate.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IBaseService _baseService;

        public IndexModel(
            IUserService userService,
            IBaseService baseService
            )
        {
            _userService = userService;
            _baseService = baseService;
        }

        [BindProperty]
        public UserLoginViewModel Input { get; set; }

        [ViewData]
        public string ErrorMessage { get; set; }

        public IActionResult OnGet(string returnUrl)
        {
            //            var local = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
            //            if (local)
            //                await _userService.RegisterFirstUserAsync().ConfigureAwait(false);

            Input = new UserLoginViewModel();

            if (!string.IsNullOrEmpty(ErrorMessage))
                ModelState.AddModelError(string.Empty, ErrorMessage);

            if (User.Identity.IsAuthenticated)
            {
                if (!string.IsNullOrEmpty(Input.ReturnUrl))
                    return Redirect(Input.ReturnUrl);

                return RedirectToPage($"/{nameof(Manage)}/Index");
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                Input.ReturnUrl = returnUrl;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var status = await _userService.SignInAsync(Input).ConfigureAwait(false);

            if (status == StatusEnum.SignedIn)
            {
                if (!string.IsNullOrEmpty(Input.ReturnUrl))
                    return Redirect(Input.ReturnUrl);

                return RedirectToPage(typeof(Manage.IndexModel).Page());
            }

            ErrorMessage = status.GetDisplayName();
            return Page();
        }
    }
}