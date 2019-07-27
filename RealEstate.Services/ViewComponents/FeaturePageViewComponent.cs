using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.ViewModels.ModelBind;

namespace RealEstate.Services.ViewComponents
{
    public class FeaturePageViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(List<FeatureViewModel> models)
        {
            return View(models);
        }
    }
}