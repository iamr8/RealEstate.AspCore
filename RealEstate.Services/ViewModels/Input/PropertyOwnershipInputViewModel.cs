using RealEstate.Base;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels.Input
{
    public class PropertyOwnershipInputViewModel : BaseInputViewModel
    {
        public string PropertyId { get; set; }
        public string OwnersJson { get; private set; }

        public List<OwnershipInputViewModel> Owners
        {
            get => OwnersJson.JsonGetAccessor<List<OwnershipInputViewModel>>();
            set => OwnersJson = value.JsonSetAccessor();
        }
    }
}