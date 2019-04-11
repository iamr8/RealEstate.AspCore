using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.ViewModels.Component;

namespace RealEstate.Web.Components
{
    public class PropertyCreatorViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(PropertyCreatorComponentModel model)
        {
            return View(model ?? new PropertyCreatorComponentModel());
        }
    }
}