using RealEstate.Base;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels.Input
{
    public class PropertyOwnershipInputViewModel : BaseInputViewModel
    {
        private string _ownersJson;
        public string PropertyId { get; set; }

        public string OwnersJson
        {
            get => _ownersJson;
            set => _ownersJson = value.JsonSetAccessor();
        }

        public List<OwnershipInputViewModel> Owners
        {
            get => OwnersJson.JsonGetAccessor<List<OwnershipInputViewModel>>();
            set => OwnersJson = value.JsonSetAccessor();
        }
    }
}