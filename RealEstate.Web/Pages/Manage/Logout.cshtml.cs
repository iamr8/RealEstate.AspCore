using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RealEstate.Services;
using System.Threading.Tasks;
using RealEstate.Services.ServiceLayer;

namespace RealEstate.Web.Pages.Manage
{
    [Authorize]
    public class LogoutModel : PageModel
    {
        private readonly IUserService _userService;

        public LogoutModel(
            IUserService userService
            )
        {
            _userService = userService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await _userService.SignOutAsync().ConfigureAwait(false);
            return RedirectToPage("/Index");
        }
    }
}