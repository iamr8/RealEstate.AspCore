using System.Collections.Generic;
using Newtonsoft.Json;
using RealEstate.Base;

namespace RealEstate.Services.ViewModels.Input
{
    public class PropertyOwnershipInputViewModel : BaseInputViewModel
    {
        public string PropertyId { get; set; }
        public string OwnersJson { get; set; }

        public List<OwnershipInputViewModel> Owners =>
            string.IsNullOrEmpty(OwnersJson)
                ? default
                : JsonConvert.DeserializeObject<List<OwnershipInputViewModel>>(OwnersJson);
    }
}