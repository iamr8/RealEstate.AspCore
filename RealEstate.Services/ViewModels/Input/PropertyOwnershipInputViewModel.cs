using Newtonsoft.Json;
using RealEstate.Base;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels.Input
{
    public class PropertyOwnershipInputViewModel : BaseInputViewModel
    {
        public string PropertyId { get; set; }
        public string OwnersJson { get; set; }

        public List<OwnershipInputViewModel> Owners
        {
            get => string.IsNullOrEmpty(OwnersJson)
                ? default
                : JsonConvert.DeserializeObject<List<OwnershipInputViewModel>>(OwnersJson);
            set => OwnersJson = JsonConvert.SerializeObject(value);
        }
    }
}