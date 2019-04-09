using RealEstate.Base;
using RealEstate.Domain.Tables;

namespace RealEstate.ViewModels
{
    public class FeatureValueViewModel : BaseLogViewModel<ApplicantFeature>
    {
        public FeatureValueViewModel()
        {
        }

        public FeatureValueViewModel(ApplicantFeature entity) : base(entity)
        {
            if (entity == null)
                return;

            Id = Entity.Id;
            Value = Entity.Value;
        }

        public string Value { get; set; }
        public FeatureViewModel Feature { get; set; }
    }
}