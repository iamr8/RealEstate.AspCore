using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using RealEstate.Base;
using RealEstate.Base.Api;
using RealEstate.Base.Enums;
using RealEstate.Resources;
using RealEstate.Services.ServiceLayer;
using RealEstate.Services.ViewModels.ModelBind;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using JsonExtensions = RealEstate.Base.JsonExtensions;

namespace RealEstate.Services
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAsyncActionFilter
    {
        private bool AllowAnonymous { get; }
        private double MinimumAppVersion { get; }
        private string[] AllowedOsVersions { get; set; }
        private string AllowedOs { get; set; }
        private ResponseStatus Response { get; }
        public UserResponse User { get; private set; }
        public Version Version { get; private set; }
        public OS UserOs { get; private set; }
        public Device Device { get; private set; }

        public AuthorizeAttribute(double minAppVersion, bool allowAnonymous, string os = null, params string[] allowedOsVersions)
        {
            AllowAnonymous = allowAnonymous;
            MinimumAppVersion = minAppVersion;

            AllowedOs = os;
            AllowedOsVersions = allowedOsVersions.Where(x => !string.IsNullOrEmpty(x)).ToArray();

            Response = new ResponseStatus
            {
                Message = StatusEnum.Forbidden.GetDisplayName(),
                Success = false
            };
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Services
            var httpContext = context.HttpContext;
            var appService = httpContext.RequestServices.GetService<IAppService>();
            var sharedLocalizer = httpContext.RequestServices.GetService<IStringLocalizer<SharedResource>>();

            // Headers
            var token = httpContext.Request.Headers["RealEstate-Token"].ToString();
            var os = httpContext.Request.Headers["RealEstate-OS"].ToString();
            var device = httpContext.Request.Headers["RealEstate-Device"].ToString();
            var version = httpContext.Request.Headers["RealEstate-Version"].ToString();

            // Json Settings
            var jsonSettings = JsonExtensions.JsonNetSetting;
            jsonSettings.PreserveReferencesHandling = PreserveReferencesHandling.None;

            #region Os

            if (!string.IsNullOrEmpty(os))
            {
                var osArray = os.Split(" - ");
                if (osArray?.Any() == true)
                {
                    UserOs = new OS
                    {
                        Name = osArray[0],
                        Version = osArray[1]
                    };
                    if (AllowedOsVersions != null && !AllowedOsVersions.Any(x => x.Equals(UserOs.Version, StringComparison.CurrentCultureIgnoreCase)))
                        goto REJECT_NOTALLOWED_OS_VERSION;

                    if (AllowedOs != null && AllowedOs != UserOs.Name)
                        goto REJECT_NOTALLOWED_OS;
                }
            }

            #endregion Os

            #region Device

            if (!string.IsNullOrEmpty(device))
            {
                try
                {
                    var deviceArray = device.Split(", ");
                    if (deviceArray?.Any() == true)
                    {
                        Device = new Device
                        {
                            Manufacturer = deviceArray[0],
                            Model = deviceArray[1]
                        };
                    }
                }
                catch (Exception e)
                {
                    Device = default;
                }
            }

            #endregion Device

            #region Version

            double currentAppVersion;
            try
            {
                if (string.IsNullOrEmpty(version))
                    goto REJECT_NO_VERSION;

                var versionArray = version.Split(".");
                Version = new Version(int.Parse(versionArray[0]), int.Parse(versionArray[1]), int.Parse(versionArray[2]));
                currentAppVersion = double.Parse($"{Version.Major}.{Version.Minor}");
            }
            catch
            {
                goto REJECT_NO_VERSION;
            }

            if (currentAppVersion < MinimumAppVersion)
                goto REJECT_NO_VERSION;

            #endregion Version

            #region Token

            if (!AllowAnonymous)
            {
                List<Claim> claims;
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    if (!(handler.ReadToken(token) is JwtSecurityToken jwtSecurityToken))
                        goto REJECT_NO_TOKEN;

                    claims = jwtSecurityToken.Claims.ToList();
                }
                catch
                {
                    goto REJECT_NO_TOKEN;
                }

                if (claims?.Any() != true)
                    goto REJECT_NO_TOKEN;

                // ReSharper disable once LocalNameCapturedOnly
                UserViewModel template;
                var id = claims.FirstOrDefault(x => x.Type.Equals(nameof(template.Id), StringComparison.CurrentCulture))?.Value;
                var userName = claims.FirstOrDefault(x => x.Type.Equals(nameof(template.Username), StringComparison.CurrentCulture))?.Value;
                var password = claims.FirstOrDefault(x => x.Type.Equals(nameof(template.Password), StringComparison.CurrentCulture))?.Value;

                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                    goto REJECT_NO_TOKEN;

                var user = await appService.FindUserAsync(id, userName, password);
                if (user == null)
                    goto REJECT_NO_USER;

                Response.Message = StatusEnum.Success.GetDisplayName();
                Response.Success = true;
                User = user;
            }

            #endregion Token

            if (!Response.Success)
                goto REJECT_RESPONSE;

            await next();

            REJECT_NO_TOKEN:
            Response.Message = StatusEnum.Forbidden.GetDisplayName();
            goto REJECT_RESPONSE;

            REJECT_NO_USER:
            Response.Message = StatusEnum.UserNotFound.GetDisplayName();
            goto REJECT_RESPONSE;

            REJECT_NO_VERSION:
            Response.Message = sharedLocalizer[SharedResource.UpdateApplicationToUse];
            goto REJECT_RESPONSE;

            REJECT_NOTALLOWED_OS_VERSION:
            Response.Message = sharedLocalizer[SharedResource.OSVersionNotSupported];
            goto REJECT_RESPONSE;

            REJECT_NOTALLOWED_OS:
            Response.Message = sharedLocalizer[SharedResource.OSNotSupported];
            goto REJECT_RESPONSE;

            REJECT_RESPONSE:
            Response.Success = false;
            User = null;
            context.Result = new JsonResult(Response, jsonSettings);
        }
    }
}