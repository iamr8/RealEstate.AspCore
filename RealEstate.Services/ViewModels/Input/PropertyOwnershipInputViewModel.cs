using System.Collections.Generic;
using RealEstate.Base;

namespace RealEstate.Services.ViewModels.Input
{
    public class PropertyOwnershipInputViewModel : BaseInputViewModel
    {
        private string _ownersJson;
        public string PropertyId { get; set; }

        public string OwnersJson
        {
            get => _ownersJson;
            set => _ownersJson = JsonExtensions.InitJson(value);
        }

        public List<OwnershipInputViewModel> Owners
        {
            get => JsonExtensions.Deserialize<List<OwnershipInputViewModel>>(OwnersJson);
            set => OwnersJson = value.Serialize();
        }
    }
}