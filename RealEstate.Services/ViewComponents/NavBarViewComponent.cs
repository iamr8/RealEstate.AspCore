using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.Extensions;

namespace RealEstate.Services.ViewComponents
{
    public class NavBarViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View(Request.IsFromApp());
        }
    }
}