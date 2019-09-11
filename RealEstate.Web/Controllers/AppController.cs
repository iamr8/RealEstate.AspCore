using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using RealEstate.Base;
using RealEstate.Base.Api;
using RealEstate.Resources;
using RealEstate.Services;
using RealEstate.Services.ServiceLayer;
using RealEstate.Services.ViewModels.Api.Request;

namespace RealEstate.Web.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}")]
    [Produces("application/json")]
    [ApiController]
    public class AppController : ControllerBase
    {
        private readonly IAppService _appService;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly JsonSerializerSettings _settings;

        public AppController(
            IAppService appService,
            IStringLocalizer<SharedResource> localizer
            )
        {
            _appService = appService;
            _localizer = localizer;

            var tempSettings = JsonExtensions.JsonNetSetting;
            tempSettings.PreserveReferencesHandling = PreserveReferencesHandling.None;
            _settings = tempSettings;
        }

        private AuthorizeStatus AuthorizeStatus
        {
            get
            {
                try
                {
                    if (!(ControllerContext.ActionDescriptor.EndpointMetadata?.FirstOrDefault(x => x.GetType() == typeof(AuthorizeAttribute)) is AuthorizeAttribute
                        permission))
                        return new AuthorizeStatus
                        {
                            Success = false,
                            Message = _localizer[SharedResource.UnexpectedError]
                        };

                    return permission.Status;
                }
                catch
                {
                    return new AuthorizeStatus
                    {
                        Success = false,
                        Message = _localizer[SharedResource.UnexpectedError]
                    };
                }
            }
        }

        [Route("signin"), HttpPost]
        [MapToApiVersion("1"), Authorize(1.0, true)]
        public async Task<IActionResult> SignInAsync([FromForm] SignInRequest model)
        {
            var response = await _appService.SignInAsync(model);
            return new JsonResult(response, _settings);
        }

        [Route("config"), HttpGet]
        [MapToApiVersion("1"), Authorize(1.0)]
        public async Task<IActionResult> ConfigAsync()
        {
            var response = await _appService.ConfigAsync(this.AuthorizeStatus.User.UserId);
            return new JsonResult(response, _settings);
        }

        [Route("items"), HttpPost]
        [MapToApiVersion("1"), Authorize(1.0)]
        public async Task<IActionResult> ItemsAsync([FromForm] ItemRequest model)
        {
            var response = await _appService.ItemListAsync(model, this.AuthorizeStatus.User.UserId);
            return new JsonResult(response, _settings);
        }

        [Route("reminders"), HttpPost]
        [MapToApiVersion("1"), Authorize(1.0)]
        public async Task<IActionResult> RemindersAsync([FromForm] ReminderRequest model)
        {
            var response = await _appService.RemindersAsync(model, this.AuthorizeStatus.User.UserId);
            return new JsonResult(response, _settings);
        }

        [Route("zoonkans"), HttpGet]
        [MapToApiVersion("1"), Authorize(1.0)]
        public async Task<IActionResult> ZoonkansAsync()
        {
            var response = await _appService.ZoonkansAsync();
            return new JsonResult(response, _settings);
        }
    }
}