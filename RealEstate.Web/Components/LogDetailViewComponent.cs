using Microsoft.AspNetCore.Mvc;
using RealEstate.Base;

namespace RealEstate.Web.Components
{
    public class LogDetailViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(LogViewModel tracker)
        {
            return View(tracker ?? new LogViewModel());
        }
    }
}