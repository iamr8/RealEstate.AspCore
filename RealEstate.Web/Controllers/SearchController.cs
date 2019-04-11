using Microsoft.AspNetCore.Mvc;
using RealEstate.Services;
using System.Threading.Tasks;

namespace RealEstate.Web.Controllers
{
    [Route("manage")]
    public class SearchController : Controller
    {
        private readonly IPropertyService _propertyService;

        public SearchController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        [Route("property/search")]
        public async Task<IActionResult> PropertyAsync(string term)
        {
            var model = await _propertyService.PropertyListAsync(term).ConfigureAwait(false);
            return new JsonResult(model);
        }
    }
}