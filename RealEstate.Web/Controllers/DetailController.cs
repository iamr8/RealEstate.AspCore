using Microsoft.AspNetCore.Mvc;
using RealEstate.Services;
using System.Threading.Tasks;

namespace RealEstate.Web.Controllers
{
    [Route("manage")]
    [ApiController]
    public class DetailController : ControllerBase
    {
        private readonly IContactService _contactService;
        private readonly IPropertyService _propertyService;

        public DetailController(
            IContactService contactService,
            IPropertyService propertyService
            )
        {
            _contactService = contactService;
            _propertyService = propertyService;
        }

        [Route("ownership/detail")]
        public async Task<IActionResult> OwnershipAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return new JsonResult(null);

            var models = await _contactService.OwnershipJsonAsync(id).ConfigureAwait(false);
            return new JsonResult(models);
        }

        [Route("property/detail")]
        public async Task<IActionResult> PropertyAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return new JsonResult(null);

            var models = await _propertyService.PropertyJsonAsync(id).ConfigureAwait(false);
            return new JsonResult(models);
        }
    }
}