using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.ServiceLayer;
using RealEstate.Services.ViewModels.Api.Request;
using System.Threading.Tasks;

namespace RealEstate.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppController : ControllerBase
    {
        private readonly IItemService _itemService;
        private readonly IUserService _userService;

        public AppController(
            IItemService itemService,
            IUserService userService
            )
        {
            _itemService = itemService;
            _userService = userService;
        }

        [Route("signin"), HttpPost]
        public async Task<IActionResult> SignInAsync([FromForm] SignInRequest model)
        {
            var response = await _userService.SignInAsync(model);
            return new JsonResult(response);
        }

        [Route("items"), HttpPost]
        public async Task<IActionResult> ItemsAsync([FromForm] ItemRequest model)
        {
            var items = await _itemService.ItemListAsync(model);
            return new JsonResult(items);
        }
    }
}