using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.BaseLog;

namespace RealEstate.Services.ViewComponents
{
    public class LogViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(LogViewModel logs, string cat, string id)
        {
            if (logs == null)
                return View(new BaseLogWrapperViewModel());

            var model = new BaseLogWrapperViewModel
            {
                Id = id,
                Entity = cat,
                Modifies = logs.Modifies,
                Create = logs.Create,
                Deletes = logs.Deletes
            };
            return View(model);
        }
    }
}