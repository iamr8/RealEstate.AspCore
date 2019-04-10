using System.Collections.Generic;
using RealEstate.Base;

namespace RealEstate.Services.ViewModels
{
    public class PropertyOwnershipViewModel : BaseViewModel
    {
        public List<OwnershipViewModel> Owners { get; set; }
    }
}