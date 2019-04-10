using Microsoft.AspNetCore.Authentication.Cookies;

namespace RealEstate.Services.Extensions
{
    public static class AuthenticationScheme
    {
        public static string Scheme => CookieAuthenticationDefaults.AuthenticationScheme;
    }
}