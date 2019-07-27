using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.ViewModels.ModelBind;

namespace RealEstate.Services.ViewComponents
{
    public class FacilityPageViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(List<FacilityViewModel> models)
        {
            return View(models);
        }
    }
}