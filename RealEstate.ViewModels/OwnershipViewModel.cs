using RealEstate.Base;
using System.Collections.Generic;

namespace RealEstate.ViewModels
{
    public class OwnershipViewModel : BaseViewModel
    {
        public List<OwnerViewModel> Owners { get; set; }
        public string PropertyId { get; set; }
    }
}