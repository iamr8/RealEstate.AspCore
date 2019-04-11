using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.ViewModels;
using RealEstate.Services.ViewModels.Component;

namespace RealEstate.Web.Components
{
    public class PersonSelectorViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(SelectorComponentModel model)
        {
            return View(model ?? new SelectorComponentModel());
        }
    }
}