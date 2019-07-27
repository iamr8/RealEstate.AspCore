using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.ViewModels.ModelBind;

namespace RealEstate.Services.ViewComponents
{
    public class ApplicantPageViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(List<ApplicantViewModel> models)
        {
            return View(models);
        }
    }
}