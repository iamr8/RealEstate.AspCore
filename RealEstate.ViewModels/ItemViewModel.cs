using System.Collections.Generic;
using RealEstate.Base;

namespace RealEstate.ViewModels
{
    public class ItemViewModel : BaseTrackViewModel
    {
        public string Description { get; set; }

        public bool IsRequested { get; set; }
        public ItemCategoryViewModel Category { get; set; }
        public PropertyViewModel Property { get; set; }
        public List<FeatureValueViewModel> Features { get; set; }
    }
}