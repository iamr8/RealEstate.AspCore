using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.ViewModels.ModelBind;

namespace RealEstate.Services.ViewComponents
{
    public class ItemPageViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(List<ItemViewModel> models)
        {
            return View(models);
        }
    }
}