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
        private readonly ILocationService _locationService;

        public RemoveController(
            IUserService userService,
            IFeatureService featureService,
            ILocationService locationService

            )
        {
            _userService = userService;
            _locationService = locationService;
            _featureService = featureService;
        }

        [Route("feature/remove")]
        public async Task<IActionResult> FeatureAsync(string id)
        {
            var model = await _featureService.FeatureRemoveAsync(id).ConfigureAwait(false);
            return RedirectToPage(typeof(Pages.Manage.Feature.IndexModel).Page(), new
            {
                status = (int)model
            });
        }

        [Route("district/remove")]
        public async Task<IActionResult> DistrictAsync(string id)
        {
            var model = await _locationService.DistrictRemoveAsync(id).ConfigureAwait(false);
            return RedirectToPage(typeof(Pages.Manage.District.IndexModel).Page(), new
            {
                status = (int)model
            });
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