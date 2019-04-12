using JetBrains.Annotations;
using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;

namespace RealEstate.Services.ViewModels
{
    public class PropertyFeatureViewModel : BaseLogViewModel<PropertyFeature>
    {
        private string _value;

        [JsonIgnore]
        public PropertyFeature Entity { get; private set; }

        [CanBeNull]
        public readonly PropertyFeatureViewModel Instance;

        public PropertyFeatureViewModel(PropertyFeature entity, bool includeDeleted) : base(entity)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Instance = new PropertyFeatureViewModel
            {
                Entity = entity,
                Id = entity.Id,
                Value = entity.Value,
                Logs = entity.GetLogs()
            };
        }

        public PropertyFeatureViewModel()
        {
        }

        public string Value
        {
            get => _value.FixCurrency();
            set => _value = value;
        }

        public FeatureViewModel Feature { get; set; }
    }
}