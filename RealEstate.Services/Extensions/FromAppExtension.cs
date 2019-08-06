using Microsoft.AspNetCore.Http;

namespace RealEstate.Services.Extensions
{
    public static class FromAppExtension
    {
        public static bool IsFromApp(this HttpRequest request)
        {
            var userAgent = request.Headers["User-Agent"];
            var isFromApp = !string.IsNullOrEmpty(userAgent) && userAgent.Equals("RealEstate.App");

            return isFromApp;
        }
    }
}