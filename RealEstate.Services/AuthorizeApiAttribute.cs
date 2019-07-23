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
    public class AuthorizeApiAttribute : Attribute, IAsyncActionFilter
    {
        public bool AllowAnonymous { get; }
        public double MinimumAppVersion { get; }

        public VersionCheck VersionCheck { get; private set; }
        public ResponseStatus ResponseStatus { get; }
        public UserResponse UserResponse { get; private set; }

        public AuthorizeApiAttribute(double minAppVersion, bool allowAnonymous)
        {
            AllowAnonymous = allowAnonymous;
            MinimumAppVersion = minAppVersion;

            ResponseStatus = new ResponseStatus
            {
                Message = StatusEnum.Forbidden.GetDisplayName(),
                Success = false
            };

            VersionCheck = new VersionCheck
            {
                Bugfixes = 0,
                Major = 0,
                Minor = 0
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

            // Process

            #region Version

            try
            {
                if (string.IsNullOrEmpty(version))
                {
                    ResponseStatus.Message = sharedLocalizer[SharedResource.UpdateApplicationToUse];
                    ResponseStatus.Success = false;
                    UserResponse = null;
                    context.Result = new JsonResult(ResponseStatus, jsonSettings);
                    return;
                }

                var versionArray = version.Split(".");
                VersionCheck = new VersionCheck
                {
                    Major = int.Parse(versionArray[0]),
                    Minor = int.Parse(versionArray[1]),
                    Bugfixes = int.Parse(versionArray[2])
                };
            }
            catch
            {
                VersionCheck = new VersionCheck
                {
                    Bugfixes = 0,
                    Minor = 0,
                    Major = 0
                };
            }

            double currentAppVersion;
            try
            {
                currentAppVersion = double.Parse($"{VersionCheck.Major}.{VersionCheck.Minor}");
            }
            catch
            {
                currentAppVersion = 1.0;
            }

            if (currentAppVersion < MinimumAppVersion)
            {
                ResponseStatus.Message = sharedLocalizer[SharedResource.UpdateApplicationToUse];
                ResponseStatus.Success = false;
                UserResponse = null;
                context.Result = new JsonResult(ResponseStatus, jsonSettings);
                return;
            }

            #endregion Version

            #region Token

            if (!AllowAnonymous)
            {
                List<Claim> claims;
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    if (!(handler.ReadToken(token) is JwtSecurityToken jwtSecurityToken))
                    {
                        ResponseStatus.Message = StatusEnum.Forbidden.GetDisplayName();
                        ResponseStatus.Success = false;
                        UserResponse = null;
                        context.Result = new JsonResult(ResponseStatus, jsonSettings);
                        return;
                    }
                    claims = jwtSecurityToken.Claims.ToList();
                }
                catch
                {
                    ResponseStatus.Message = StatusEnum.Forbidden.GetDisplayName();
                    ResponseStatus.Success = false;
                    UserResponse = null;
                    context.Result = new JsonResult(ResponseStatus, jsonSettings);
                    return;
                }

                if (claims?.Any() != true)
                {
                    ResponseStatus.Message = StatusEnum.Forbidden.GetDisplayName();
                    ResponseStatus.Success = false;
                    UserResponse = null;
                    context.Result = new JsonResult(ResponseStatus, jsonSettings);
                    return;
                }

                var id = claims.FirstOrDefault(x => x.Type.Equals("Id", StringComparison.CurrentCulture))?.Value;
                var userName = claims.FirstOrDefault(x => x.Type.Equals("Username", StringComparison.CurrentCulture))?.Value;
                var password = claims.FirstOrDefault(x => x.Type.Equals("Password", StringComparison.CurrentCulture))?.Value;

                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                {
                    ResponseStatus.Message = StatusEnum.Forbidden.GetDisplayName();
                    ResponseStatus.Success = false;
                    UserResponse = null;
                    context.Result = new JsonResult(ResponseStatus, jsonSettings);
                    return;
                }

                var user = await appService.FindUserAsync(id, userName, password);
                if (user == null)
                {
                    ResponseStatus.Message = StatusEnum.UserNotFound.GetDisplayName();
                    ResponseStatus.Success = false;
                    UserResponse = null;
                    context.Result = new JsonResult(ResponseStatus, jsonSettings);
                    return;
                }

                ResponseStatus.Message = StatusEnum.Success.GetDisplayName();
                ResponseStatus.Success = true;
                UserResponse = user;
            }

            #endregion Token

            if (!ResponseStatus.Success)
            {
                context.Result = new JsonResult(ResponseStatus, jsonSettings);
                return;
            }

            await next();
        }
    }
}