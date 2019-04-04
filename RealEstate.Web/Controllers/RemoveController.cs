using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Services;
using RealEstate.Web.Pages.Manage.User;
using System.Threading.Tasks;

namespace RealEstate.Web.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class RemoveController : Controller
    {
        private readonly IFeatureService _featureService;
        private readonly IUserService _userService;

        public RemoveController(
            IUserService userService,
            IFeatureService featureService
            )
        {
            _userService = userService;
            _featureService = featureService;
        }

        [Route("category/remove")]
        public async Task<IActionResult> CategoryAsync(string id)
        {
            var model = await _featureService.CategoryRemoveAsync(id).ConfigureAwait(false);
            return RedirectToPage(typeof(Pages.Manage.Category.IndexModel).Page(), new
            {
                status = (int)model
            });
        }

        [Route("user/deactivate")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> UserDeActivateAsync(string id)
        {
            var model = await _userService.RemoveAsync(id).ConfigureAwait(false);
            return RedirectToPage(typeof(IndexModel).Page(), new
            {
                status = (int)model
            });
        }
    }
}