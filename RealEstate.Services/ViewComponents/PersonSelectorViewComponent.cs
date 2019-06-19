using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.ViewModels.Component;

namespace RealEstate.Services.ViewComponents
{
    public class PersonSelectorViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(SelectorComponentModel model)
        {
            return View(model ?? new SelectorComponentModel());
        }
    }
}