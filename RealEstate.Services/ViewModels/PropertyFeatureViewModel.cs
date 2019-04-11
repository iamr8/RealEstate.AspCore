using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;

namespace RealEstate.Services.ViewModels
{
    public class PropertyFeatureViewModel : BaseLogViewModel<PropertyFeature>
    {
        public PropertyFeatureViewModel(PropertyFeature entity) : base(entity)
        {
            Id = entity.Id;
            Value = entity.Value;
        }

        public PropertyFeatureViewModel()
        {
        }

        public string Value { get; set; }
        public FeatureViewModel Feature { get; set; }
    }
}