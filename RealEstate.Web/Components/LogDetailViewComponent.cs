using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.BaseLog;

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