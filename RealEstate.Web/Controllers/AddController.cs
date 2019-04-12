using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using RealEstate.Base;
using RealEstate.Resources;
using RealEstate.Services;
using RealEstate.Services.ViewModels.Input;
using System.Threading.Tasks;

namespace RealEstate.Web.Controllers
{
    [Route("manage")]
    [ApiController]
    public class AddController : ControllerBase
    {
        private readonly IContactService _contactService;
        private readonly IPropertyService _propertyService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public AddController(
            IContactService contactService,
            IPropertyService propertyService,
            IStringLocalizer<SharedResource> localizer)
        {
            _contactService = contactService;
            _propertyService = propertyService;
            _localizer = localizer;
        }

        [Route("contact/addItem"), HttpPost]
        public async Task<IActionResult> ContactAsync([FromForm] ContactInputViewModel model)
        {
            var (status, newContact) = await _contactService.ContactAddAsync(model, false, true).ConfigureAwait(false);
            return new JsonResult(new JsonStatusValueViewModel
            {
                Status = status,
                Id = newContact?.Id,
                Name = newContact != null ? $"{newContact.Name} • {newContact.MobileNumber}" : null
            });
        }

        [Route("property/addItem"), HttpPost]
        public async Task<IActionResult> PropertyAsync([FromForm] PropertyInputViewModel model)
        {
            var (status, newProperty) = await _propertyService.PropertyAddOrUpdateAsync(model, true).ConfigureAwait(false);
            return new JsonResult(new JsonStatusViewModel
            {
                Status = status,
                Id = newProperty?.Id,
            });
        }
    }
}