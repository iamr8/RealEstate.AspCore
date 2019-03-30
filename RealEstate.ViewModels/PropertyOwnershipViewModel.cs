using RealEstate.Base;
using System.Collections.Generic;

namespace RealEstate.ViewModels
{
    public class PropertyOwnershipViewModel : BaseViewModel
    {
        public List<OwnershipViewModel> Owners { get; set; }
    }
}