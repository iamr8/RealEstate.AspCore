using RealEstate.Base;
using RealEstate.Domain.Tables;
using System.Collections.Generic;

namespace RealEstate.ViewModels
{
    public class ItemViewModel : BaseLogViewModel<Item>
    {
        public string Description { get; set; }

        public bool IsRequested { get; set; }
        public CategoryViewModel Category { get; set; }
        public PropertyViewModel Property { get; set; }
        public List<FeatureValueViewModel> Features { get; set; }
    }
}