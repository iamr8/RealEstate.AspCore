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
        private readonly IStringLocalizer<SharedResource> _localizer;

        public AddController(
            IContactService contactService,
            IStringLocalizer<SharedResource> localizer)
        {
            _contactService = contactService;
            _localizer = localizer;
        }

        [Route("person/addItem"), HttpPost]
        public async Task<IActionResult> OwnershipAsync([FromForm] OwnershipInputViewModel model)
        {
            var (status, newOwnership) = await _contactService.OwnershipAddAsync(model, true).ConfigureAwait(false);
            return new JsonResult(new JsonStatusValueViewModel
            {
                Status = status,
                Id = newOwnership?.Id,
                Name = newOwnership != null ? $"{newOwnership.Name} • {newOwnership.Contact.MobileNumber}" : null
            });
        }
    }
}