using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace RealEstate.Extensions
{
    public static class AuthenticationScheme
    {
        public static string Scheme => CookieAuthenticationDefaults.AuthenticationScheme;
    }
}
