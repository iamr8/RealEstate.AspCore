using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RealEstate.Base;
using RealEstate.Services;
using RealEstate.Services.Extensions;
using RealEstate.Services.ServiceLayer;
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

        private readonly JsonSerializerSettings _settings;

        public AppController(IAppService appService)
        {
            _appService = appService;
            var tempSettings = JsonExtensions.JsonNetSetting;
            tempSettings.PreserveReferencesHandling = PreserveReferencesHandling.None;
            _settings = tempSettings;
        }

        [Route("signin"), HttpPost]
        [MapToApiVersion("1"), CheckPermission(1.0)]
        public async Task<IActionResult> SignInAsync([FromForm] SignInRequest model)
        {
            var response = await _appService.SignInAsync(model);
            return new JsonResult(response, _settings);
        }

        [Route("config"), HttpPost]
        [MapToApiVersion("1"), CheckPermission(1.0, true)]
        public async Task<IActionResult> ConfigAsync()
        {
            var user = ControllerContext.ActionDescriptor.EndpointMetadata.GetUserOfToken();
            var response = await _appService.ConfigAsync(user.UserId, user.Username, user.Role, user.FirstName, user.LastName, user.MobileNumber);
            return new JsonResult(response, _settings);
        }

        [Route("items"), HttpPost]
        [MapToApiVersion("1"), CheckPermission(1.0, true)]
        public async Task<IActionResult> ItemsAsync([FromForm] ItemRequest model)
        {
            var user = ControllerContext.ActionDescriptor.EndpointMetadata.GetUserOfToken();
            var response = await _appService.ItemListAsync(model, user.UserId);
            return new JsonResult(response, _settings);
        }

        [Route("reminders"), HttpPost]
        [MapToApiVersion("1"), CheckPermission(1.0, true)]
        public async Task<IActionResult> RemindersAsync([FromForm] ReminderRequest model)
        {
            var user = ControllerContext.ActionDescriptor.EndpointMetadata.GetUserOfToken();
            var response = await _appService.RemindersAsync(model, user.UserId);
            return new JsonResult(response, _settings);
        }

        [Route("zoonkans"), HttpPost]
        [MapToApiVersion("1"), CheckPermission(1.0, true)]
        public async Task<IActionResult> ZoonkansAsync()
        {
            var response = await _appService.ZoonkansAsync();
            return new JsonResult(response, _settings);
        }
    }
}