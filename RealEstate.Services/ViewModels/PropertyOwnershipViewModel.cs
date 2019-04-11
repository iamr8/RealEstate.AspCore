using RealEstate.Base;
using System;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels
{
    public class PropertyOwnershipViewModel : BaseViewModel
    {
        public DateTime DateTime { get; set; }
        public List<OwnershipViewModel> Owners { get; set; }
    }
}