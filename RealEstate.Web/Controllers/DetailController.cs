using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Services;

namespace RealEstate.Web.Controllers
{
    [Route("manage")]
    [ApiController]
    public class DetailController : ControllerBase
    {
        private readonly IContactService _contactService;

        public DetailController(
            IContactService contactService
            )
        {
            _contactService = contactService;
        }

        [Route("ownership/detail")]
        public async Task<IActionResult> OwnershipAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return new JsonResult(null);

            var models = await _contactService.OwnershipJsonAsync(id).ConfigureAwait(false);
            return new JsonResult(models);
        }
    }
}