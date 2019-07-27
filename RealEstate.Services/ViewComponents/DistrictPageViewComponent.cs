using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.ViewModels.ModelBind;

namespace RealEstate.Services.ViewComponents
{
    public class DistrictPageViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(List<DistrictViewModel> models)
        {
            return View(models);
        }
    }
}