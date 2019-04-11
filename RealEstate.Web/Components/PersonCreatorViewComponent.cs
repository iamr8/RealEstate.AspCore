﻿using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.ViewModels.Component;

namespace RealEstate.Web.Components
{
    public class PersonCreatorViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(PersonCreatorComponentModel model)
        {
            return View(model ?? new PersonCreatorComponentModel());
        }
    }
}