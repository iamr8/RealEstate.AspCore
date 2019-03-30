using RealEstate.Base;

namespace RealEstate.ViewModels
{
    public class FeatureValueViewModel : BaseTrackViewModel
    {
        public string Value { get; set; }
        public FeatureViewModel Feature { get; set; }
    }
}