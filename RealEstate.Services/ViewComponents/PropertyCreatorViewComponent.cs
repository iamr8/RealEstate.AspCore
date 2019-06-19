using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.ViewModels.Component;

namespace RealEstate.Services.ViewComponents
{
    public class PropertyCreatorViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(PropertyCreatorComponentModel model)
        {
            return View(model ?? new PropertyCreatorComponentModel());
        }
    }
}