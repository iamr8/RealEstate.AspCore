using System.Collections.Generic;
using RealEstate.Base;

namespace RealEstate.ViewModels
{
    public class ItemViewModel : BaseLogViewModel
    {
        public string Description { get; set; }

        public bool IsRequested { get; set; }
        public CategoryViewModel Category { get; set; }
        public PropertyViewModel Property { get; set; }
        public List<FeatureValueViewModel> Features { get; set; }
    }
}