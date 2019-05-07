using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;

namespace RealEstate.Services.ViewModels
{
    public class PropertyFeatureViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public PropertyFeature Entity { get; }

        public PropertyFeatureViewModel(PropertyFeature entity, bool includeDeleted, Action<PropertyFeatureViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Entity = entity;
            action?.Invoke(this);
        }

        public string Value => Entity.Value;

        public void GetProperty(bool includeDeleted = false, Action<PropertyViewModel> action = null)
        {
            Property = Entity?.Property.Into(includeDeleted, action);
        }

        public void GetFeature(bool includeDeleted = false, Action<FeatureViewModel> action = null)
        {
            Feature = Entity?.Feature.Into(includeDeleted, action);
        }

        public PropertyViewModel Property { get; private set; }
        public FeatureViewModel Feature { get; set; }
    }
}