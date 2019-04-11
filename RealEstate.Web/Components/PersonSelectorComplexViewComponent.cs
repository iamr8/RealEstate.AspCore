using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.Extensions;

namespace RealEstate.Web.Components
{
    public class PersonSelectorComplexViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(PersonSelectorComplexComponentModel model)
        {
            return View(model ?? new PersonSelectorComplexComponentModel());
        }
    }
}