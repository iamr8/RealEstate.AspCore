using RealEstate.Base;

namespace RealEstate.ViewModels
{
    public class FeatureValueViewModel : BaseLogViewModel
    {
        public string Value { get; set; }
        public FeatureViewModel Feature { get; set; }
    }
}