using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Services;
using System.Threading.Tasks;

namespace RealEstate.Web.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class RemoveController : Controller
    {
        private readonly IUserService _userService;

        public RemoveController(
            IUserService userService
            )
        {
            _userService = userService;
        }

        [Route("user/deactivate")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> UserDeActivateAsync(string id)
        {
            var model = await _userService.RemoveAsync(id).ConfigureAwait(false);
            return RedirectToPage($"/{nameof(Pages.Manage)}/{nameof(Pages.Manage.User)}/Index", new
            {
                status = (int)model
            });
        }
    }
}