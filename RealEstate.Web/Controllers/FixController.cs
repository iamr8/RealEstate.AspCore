using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.ServiceLayer;
using RealEstate.Web.Pages.Manage.Settings;
using System.Threading.Tasks;

namespace RealEstate.Web.Controllers
{
    [Route("manage")]
    public class FixController : Controller
    {
        private readonly IGlobalService _globalService;

        public FixController(IGlobalService globalService)
        {
            _globalService = globalService;
        }

        [Route("settings/fixbuildyear")]
        public async Task<IActionResult> FixBuildYears()
        {
            await _globalService.FixBuildYearAsync();
            return RedirectToPage(typeof(IndexModel).Page());
        }

        [Route("settings/fixaddress")]
        public async Task<IActionResult> FixAddress()
        {
            await _globalService.FixAddressAsync();
            return RedirectToPage(typeof(IndexModel).Page());
        }

        [Route("settings/fixfinalprice")]
        public async Task<IActionResult> FixFinalPrice()
        {
            await _globalService.FixFinalPriceAsync();
            return RedirectToPage(typeof(IndexModel).Page());
        }

        [Route("settings/fixloanprice")]
        public async Task<IActionResult> FixLoanPrice()
        {
            await _globalService.FixLoanPriceAsync();
            return RedirectToPage(typeof(IndexModel).Page());
        }
    }
}