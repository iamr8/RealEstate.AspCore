using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.ViewModels.ModelBind;

namespace RealEstate.Services.ViewComponents
{
    public class CustomerPageViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(List<CustomerViewModel> models)
        {
            return View(models);
        }
    }
}