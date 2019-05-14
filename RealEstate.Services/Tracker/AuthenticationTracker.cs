using Microsoft.AspNetCore.Authentication.Cookies;
using System.Linq;
using System.Threading.Tasks;
using RealEstate.Services.ServiceLayer;

namespace RealEstate.Services.Tracker
{
    public class AuthenticationTracker : CookieAuthenticationEvents
    {
        private readonly IUserService _userService;

        public AuthenticationTracker(
            IUserService userService
            )
        {
            _userService = userService;
        }

        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            var claims = context.Principal.Claims.ToList();
            if (claims.Count == 0) return;

            var isUserValid = await _userService.IsUserValidAsync(claims).ConfigureAwait(false);
            if (!isUserValid)
            {
                context.RejectPrincipal();
                await _userService.SignOutAsync().ConfigureAwait(false);
            }
        }
    }
}