using Microsoft.AspNetCore.Mvc;
using RealEstate.Base;
using System.Collections.Generic;

namespace RealEstate.Web.Components
{
    public class LogViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(List<LogViewModel> logs)
        {
            return View(logs ?? new List<LogViewModel>());
        }
    }
}