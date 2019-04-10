using RealEstate.Domain.Tables;
using RealEstate.Services.BaseLog;

namespace RealEstate.Services.ViewModels
{
    public class FeatureValueViewModel : BaseLogViewModel<ApplicantFeature>
    {
        public FeatureValueViewModel()
        {
        }

        public FeatureValueViewModel(ApplicantFeature entity, bool showDeleted) : base(entity)
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