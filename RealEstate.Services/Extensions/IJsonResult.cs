using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RealEstate.Base;

namespace RealEstate.Services.Extensions
{
    public class IJsonResult : IActionResult
    {
        public Task ExecuteResultAsync(ActionContext context)
        {
            var tempSettings = JsonExtensions.JsonNetSetting;
            tempSettings.PreserveReferencesHandling = PreserveReferencesHandling.None;
            throw new NotImplementedException();
        }
    }
}
