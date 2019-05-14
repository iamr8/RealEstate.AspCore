using Microsoft.AspNetCore.Mvc;
using RealEstate.Services;
using System.Threading.Tasks;
using RealEstate.Services.ServiceLayer;

namespace RealEstate.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SyncController : ControllerBase
    {
        private readonly IItemService _itemService;

        public SyncController(
            IItemService itemService
            )
        {
            _itemService = itemService;
        }

        [Route("items"), HttpPost]
        public async Task<IActionResult> ItemsAsync([FromForm] string user, [FromForm] string pass, [FromForm] string itmCategory, [FromForm] string propCategory)
        {
            var items = await _itemService.ItemListAsync(user, pass, itmCategory, propCategory).ConfigureAwait(false);
            return new JsonResult(items);
        }
    }
}