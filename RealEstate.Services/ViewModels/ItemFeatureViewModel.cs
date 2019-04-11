using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;

namespace RealEstate.Services.ViewModels
{
    public class ItemFeatureViewModel : BaseLogViewModel<ItemFeature>
    {
        public ItemFeatureViewModel(ItemFeature entity) : base(entity)
        {
            Id = entity.Id;
            Value = entity.Value;
        }

        public ItemFeatureViewModel()
        {
        }

        public string Value { get; set; }
        public FeatureViewModel Feature { get; set; }
    }
}