using RealEstate.Base;
using RealEstate.Base.Enums;

namespace RealEstate.ViewModels
{
    public class FeatureViewModel : BaseLogViewModel
    {
        public string Name { get; set; }

        public FeatureTypeEnum Type { get; set; }

        public int Properties { get; set; }
    }
}