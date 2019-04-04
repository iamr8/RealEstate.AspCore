using Microsoft.AspNetCore.Mvc;
using RealEstate.Base;

namespace RealEstate.Web.Components
{
    public class LogDetailViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(LogDetailViewModel tracker)
        {
            return View(tracker ?? new LogDetailViewModel());
        }
    }
}