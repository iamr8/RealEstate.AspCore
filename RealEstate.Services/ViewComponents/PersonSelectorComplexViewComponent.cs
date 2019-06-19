using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.Extensions;

namespace RealEstate.Services.ViewComponents
{
    public class PersonSelectorComplexViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(PersonSelectorComplexComponentModel model)
        {
            return View(model ?? new PersonSelectorComplexComponentModel());
        }
    }
}