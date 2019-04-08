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
        private readonly IContactService _contactService;

        public RemoveController(
            IUserService userService,
            IFeatureService featureService,
            ILocationService locationService,
            IContactService contactService

            )
        {
            _userService = userService;
            _contactService = contactService;
            _locationService = locationService;
            _featureService = featureService;
        }

        [Route("facility/remove")]
        public async Task<IActionResult> FacilityAsync(string id)
        {
            var model = await _featureService.FacilityRemoveAsync(id).ConfigureAwait(false);
            return RedirectToPage(typeof(Pages.Manage.Facility.IndexModel).Page(), new
            {
                status = (int)model
            });
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

        [Route("contact/remove")]
        public async Task<IActionResult> ContactAsync(string id)
        {
            var model = await _contactService.ContactRemoveAsync(id).ConfigureAwait(false);
            return RedirectToPage(typeof(Pages.Manage.Contact.IndexModel).Page(), new
            {
                status = (int)model
            });
        }

        [Route("applicant/remove")]
        public async Task<IActionResult> ApplicantAsync(string id)
        {
            var model = await _contactService.ApplicantRemoveAsync(id).ConfigureAwait(false);
            return RedirectToPage(typeof(Pages.Manage.Applicant.IndexModel).Page(), new
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