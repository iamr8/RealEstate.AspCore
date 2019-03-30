using Microsoft.AspNetCore.Hosting;

namespace RealEstate.Base
{
    public static class ServerMapPathExtensions
    {
        public static string MapPath(this IHostingEnvironment environment, string path)
        {
            return $"{environment.WebRootPath}\\{path}";
        }
    }
}