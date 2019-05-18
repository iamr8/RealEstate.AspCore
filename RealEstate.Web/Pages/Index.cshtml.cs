using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RealEstate.Base.Enums;
using RealEstate.Services;
using RealEstate.Services.ViewModels.Input;
using System.Threading.Tasks;
using RealEstate.Services.ServiceLayer;
using RealEstate.Services.ServiceLayer.Base;

namespace RealEstate.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IBaseService _baseService;
        private readonly IItemService _itemService;

        public IndexModel(
            IUserService userService,
            IBaseService baseService,
            IItemService itemService
            )
        {
            _userService = userService;
            _baseService = baseService;
            _itemService = itemService;
        }

        [BindProperty]
        public UserLoginViewModel Input { get; set; }

        public StatusEnum Status { get; set; }

        public IActionResult OnGet(string returnUrl, string status)
        {
            Input = new UserLoginViewModel();

            if (User.Identity.IsAuthenticated)
            {
                if (!string.IsNullOrEmpty(Input.ReturnUrl))
                    return Redirect(Input.ReturnUrl);

                return RedirectToPage(typeof(Manage.IndexModel).Page());
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                Input.ReturnUrl = returnUrl;

            Status = !string.IsNullOrEmpty(status) && int.TryParse(status, out var statusInt)
                ? (StatusEnum)statusInt
                : StatusEnum.Ready;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var finalStatus = ModelState.IsValid
                ? await _userService.SignInAsync(Input).ConfigureAwait(false)
                : StatusEnum.RetryAfterReview;

            if (finalStatus != StatusEnum.SignedIn)
            {
                return RedirectToPage(typeof(IndexModel).Page(), new
                {
                    status = (int)finalStatus
                });
            }

            if (!string.IsNullOrEmpty(Input.ReturnUrl))
                return Redirect(Input.ReturnUrl);

            return RedirectToPage(typeof(Manage.IndexModel).Page());
        }
    }
}