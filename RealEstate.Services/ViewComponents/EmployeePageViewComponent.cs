using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.ViewModels.ModelBind;

namespace RealEstate.Services.ViewComponents
{
    public class EmployeePageViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(List<EmployeeViewModel> models)
        {
            return View(models);
        }
    }
}