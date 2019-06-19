using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.ViewModels.Component;

namespace RealEstate.Services.ViewComponents
{
    public class PropertySelectorComplexViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(PropertySelectorComplexComponentModel model)
        {
            return View(model ?? new PropertySelectorComplexComponentModel());
        }
    }
}