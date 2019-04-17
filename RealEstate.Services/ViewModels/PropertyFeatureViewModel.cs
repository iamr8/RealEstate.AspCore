using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;

namespace RealEstate.Services.ViewModels
{
    public class PropertyFeatureViewModel : BaseLogViewModel<PropertyFeature>
    {
        [JsonIgnore]
        private readonly PropertyFeature _entity;

        public PropertyFeatureViewModel(PropertyFeature entity, bool includeDeleted, Action<PropertyFeatureViewModel> action = null) : base(entity)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            _entity = entity;
            Id = entity.Id;
            Logs = entity.GetLogs();
            action?.Invoke(this);
        }

        public string Value => _entity.Value;

        public void GetProperty(bool includeDeleted = false, Action<PropertyViewModel> action = null)
        {
            Property = _entity?.Property.Into(includeDeleted, action);
        }

        public void GetFeature(bool includeDeleted = false, Action<FeatureViewModel> action = null)
        {
            Feature = _entity?.Feature.Into(includeDeleted, action);
        }

        public PropertyViewModel Property { get; private set; }
        public FeatureViewModel Feature { get; set; }
    }
}