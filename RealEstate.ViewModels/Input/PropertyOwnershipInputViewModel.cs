using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RealEstate.Base;

namespace RealEstate.ViewModels.Input
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