using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.ViewModels.ModelBind;

namespace RealEstate.Services.ViewComponents
{
    public class ReminderPageViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(List<ReminderViewModel> models)
        {
            return View(models);
        }
    }
}