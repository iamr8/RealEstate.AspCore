using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.ViewModels.Component;

namespace RealEstate.Web.Components
{
    public class JsonSelectorViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(JsonSelectorComponentModel model)
        {
            return View(model ?? new JsonSelectorComponentModel());
        }
    }
}