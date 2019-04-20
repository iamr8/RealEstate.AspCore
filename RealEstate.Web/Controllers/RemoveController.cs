using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Services;
using RealEstate.Web.Pages.Manage.User;
using System.Threading.Tasks;

namespace RealEstate.Web.Controllers
{
    [Route("manage")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class RemoveController : Controller
    {
        private readonly IFeatureService _featureService;
        private readonly IUserService _userService;
        private readonly ILocationService _locationService;
        private readonly ICustomerService _customerService;
        private readonly IPropertyService _propertyService;
        private readonly IDivisionService _divisionService;
        private readonly IItemService _itemService;

        public RemoveController(
            IUserService userService,
            IFeatureService featureService,
            ILocationService locationService,
            ICustomerService customerService,
            IDivisionService divisionService,
            IPropertyService propertyService,
            IItemService itemService

            )
        {
            _userService = userService;
            _customerService = customerService;
            _locationService = locationService;
            _featureService = featureService;
            _propertyService = propertyService;
            _divisionService = divisionService;
            _itemService = itemService;
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

        [Route("division/remove")]
        public async Task<IActionResult> DivisionAsync(string id)
        {
            var model = await _divisionService.RemoveAsync(id).ConfigureAwait(false);
            return RedirectToPage(typeof(Pages.Manage.Division.IndexModel).Page(), new
            {
                status = (int)model
            });
        }

        [Route("item/remove")]
        public async Task<IActionResult> ItemAsync(string id)
        {
            var model = await _itemService.ItemRemoveAsync(id).ConfigureAwait(false);
            return RedirectToPage(typeof(Pages.Manage.Item.IndexModel).Page(), new
            {
                status = (int)model
            });
        }

        [Route("property/remove")]
        public async Task<IActionResult> PropertyAsync(string id)
        {
            var model = await _propertyService.PropertyRemoveAsync(id).ConfigureAwait(false);
            return RedirectToPage(typeof(Pages.Manage.Property.IndexModel).Page(), new
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

        [Route("customer/remove")]
        public async Task<IActionResult> CustomerAsync(string id)
        {
            var model = await _customerService.CustomerRemoveAsync(id).ConfigureAwait(false);
            return RedirectToPage(typeof(Pages.Manage.Customer.IndexModel).Page(), new
            {
                status = (int)model
            });
        }

        [Route("applicant/remove")]
        public async Task<IActionResult> ApplicantAsync(string id)
        {
            var model = await _customerService.ApplicantRemoveAsync(id).ConfigureAwait(false);
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