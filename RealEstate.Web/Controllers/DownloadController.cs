using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Reflection;

namespace RealEstate.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DownloadController : ControllerBase
    {
        [Route("synchronizer")]
        public IActionResult Synchronizer()
        {
            var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            const string fileName = "Synchronizer.html";
            var syncPath = $"{path}\\{fileName}";

            var syncStream = System.IO.File.OpenRead(syncPath);
            return File(syncStream, "application/octet-stream", fileName);
        }
    }
}