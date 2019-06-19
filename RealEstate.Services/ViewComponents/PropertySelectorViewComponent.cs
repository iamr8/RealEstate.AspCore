using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.ViewModels.Component;

namespace RealEstate.Services.ViewComponents
{
    public class PropertySelectorViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(PropertySelectorComponentModel model)
        {
            return View(model ?? new PropertySelectorComponentModel());
        }
    }
}