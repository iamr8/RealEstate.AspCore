using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.ViewModels.ModelBind;

namespace RealEstate.Services.ViewComponents
{
    public class UserPageViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(List<UserViewModel> models)
        {
            return View(models);
        }
    }
}