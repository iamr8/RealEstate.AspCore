using Microsoft.AspNetCore.Mvc;
using RealEstate.ViewModels.Input;

namespace RealEstate.Web.Components
{
    public class PersonCreatorViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ContactInputViewModel model)
        {
            return View(model ?? new ContactInputViewModel());
        }
    }
}