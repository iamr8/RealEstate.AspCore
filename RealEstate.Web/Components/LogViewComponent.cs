using Microsoft.AspNetCore.Mvc;
using RealEstate.Base;

namespace RealEstate.Web.Components
{
    public class LogViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(LogViewModel logs)
        {
            return View(logs ?? new LogViewModel());
        }
    }
}