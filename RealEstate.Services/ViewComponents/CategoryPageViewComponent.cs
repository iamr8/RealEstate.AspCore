using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.ViewModels.ModelBind;

namespace RealEstate.Services.ViewComponents
{
    public class CategoryPageViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(List<CategoryViewModel> models)
        {
            return View(models);
        }
    }
}