using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.ServiceLayer;
using RealEstate.Services.ViewModels.Api;
using RealEstate.Services.ViewModels.Api.Request;
using System.Threading.Tasks;

namespace RealEstate.Web.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}")]
    [ApiController]
    public class AppController : ControllerBase
    {
        private readonly IAppService _appService;

        public AppController(
            IAppService appService
            )
        {
            _appService = appService;
        }

        [Route("signin")]
        [HttpPost, MapToApiVersion("1")]
        public async Task<IActionResult> SignInAsync([FromForm] SignInRequest model)
        {
            var response = await _appService.SignInAsync(model);
            return new JsonResult(response);
        }

        [Route("config")]
        [HttpPost, MapToApiVersion("1")]
        public async Task<IActionResult> ConfigAsync([FromForm] RequestWrapper model)
        {
            var response = await _appService.ConfigAsync(model);
            return new JsonResult(response);
        }

        [Route("items"), HttpPost]
        [HttpPost, MapToApiVersion("1")]
        public async Task<IActionResult> ItemsAsync([FromForm] ItemRequest model)
        {
            var response = await _appService.ItemListAsync(model);
            return new JsonResult(response);
        }
    }
}