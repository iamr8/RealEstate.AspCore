//using UAParser;

namespace RealEstate.Extensions
{
    public static class LastActivityParser
    {
        //public static string LastActivity()
        //{
        //    var request = context.Request;

        //    var ip = IpAddressWatcher.GetIpAddress ?? context.HttpContext.Connection.LocalIpAddress.ToString();
        //    var url = request.GetDisplayUrl().Replace($"{request.Scheme}://{request.Host.ToString()}", "");
        //    var userAgent = request.Headers["User-Agent"];

        //    var browser = string.Empty;
        //    var os = string.Empty;
        //    var device = string.Empty;
        //    if (!string.IsNullOrEmpty(userAgent))
        //    {
        //        var parser = Parser.GetDefault();
        //        var parsed = parser.Parse(userAgent);

        //        browser = $"{parsed.UserAgent.Family} {parsed.UserAgent.Major}";
        //        os = $"{parsed.OS.Family} {parsed.OS.Major}";
        //        device = $"{parsed.Device.Brand} {parsed.Device.Family} {(!string.IsNullOrEmpty(parsed.Device.Model) ? "(" + parsed.Device.Model + ")" : "")}";
        //    }
        //}
    }
}