using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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
using JsonExtensions = RealEstate.Base.JsonExtensions;

namespace RealEstate.Services
{
    /// <summary>
    /// Autocheck token's authorization to claims user's identity
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAsyncActionFilter
    {
        private bool AllowAnonymous { get; }
        private double MinimumAppVersion { get; }
        private string[] AllowedOsVersions { get; set; }
        private string AllowedOs { get; set; }
        public AuthorizeStatus Status { get; set; }

        public AuthorizeAttribute(double minAppVersion, bool allowAnonymous = false, OsNames os = OsNames.Everything, params string[] allowedOsVersions)
        {
            AllowAnonymous = allowAnonymous;
            MinimumAppVersion = minAppVersion;

            AllowedOs = os.ToString();
            AllowedOsVersions = allowedOsVersions.Where(x => !string.IsNullOrEmpty(x)).ToArray();

            Status = new AuthorizeStatus
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
                try
                {
                    var osArray = os.Split(" ");
                    if (osArray?.Any() == true)
                    {
                        Status.UserOs = new OS
                        {
                            Name = osArray[0],
                            Version = osArray[1]
                        };
                        if (!AllowedOs.Equals(nameof(OsNames.Everything), StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (!AllowedOs.Equals(Status.UserOs.Name, StringComparison.CurrentCultureIgnoreCase))
                                goto REJECT_NOTALLOWED_OS;

                            if (AllowedOsVersions != null)
                                if (!AllowedOsVersions.Any(x => x.Equals(Status.UserOs.Version, StringComparison.CurrentCultureIgnoreCase)))
                                    goto REJECT_NOTALLOWED_OS_VERSION;
                        }
                    }
                }
                catch
                {
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
                        Status.Device = new Device
                        {
                            Manufacturer = deviceArray[0],
                            Model = deviceArray[1]
                        };
                    }
                }
                catch (Exception)
                {
                    Status.Device = default;
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
                Status.Version = new Version(int.Parse(versionArray[0]), int.Parse(versionArray[1]), int.Parse(versionArray[2]));
                currentAppVersion = double.Parse($"{Status.Version.Major}.{Status.Version.Minor}");
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

                Status.Message = StatusEnum.Success.GetDisplayName();
                Status.Success = true;
                Status.User = user;
            }

            #endregion Token

            await next();
            return;

            REJECT_NO_TOKEN:
            Status.Message = StatusEnum.Forbidden.GetDisplayName();
            Status.Success = false;
            context.Result = new UnauthorizedObjectResult(Status);
            return;

            REJECT_NO_USER:
            Status.Message = StatusEnum.UserNotFound.GetDisplayName();
            Status.Success = false;
            context.Result = new NotFoundObjectResult(Status);
            return;

            REJECT_NO_VERSION:
            Status.Message = sharedLocalizer[SharedResource.UpdateApplicationToUse];
            goto REJECT_BADREQUEST;

            REJECT_NOTALLOWED_OS_VERSION:
            Status.Message = sharedLocalizer[SharedResource.OSVersionNotSupported];
            goto REJECT_BADREQUEST;

            REJECT_NOTALLOWED_OS:
            Status.Message = sharedLocalizer[SharedResource.OSNotSupported];
            goto REJECT_BADREQUEST;

            REJECT_BADREQUEST:
            Status.Success = false;
            context.Result = new BadRequestObjectResult(Status);
            return;
        }
    }
}