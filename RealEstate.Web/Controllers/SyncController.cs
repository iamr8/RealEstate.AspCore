using Microsoft.AspNetCore.Mvc;
using RealEstate.Services;
using System.Threading.Tasks;

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

        [Route("items")]
        public async Task<IActionResult> ItemsAsync()
        {
            var models = await _itemService.ItemListAsync().ConfigureAwait(false);
            return new JsonResult(models);
        }
    }
}