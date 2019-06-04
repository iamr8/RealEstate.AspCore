using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using System;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class PropertyFeatureViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public PropertyFeature Entity { get; }

        public PropertyFeatureViewModel(PropertyFeature entity, Action<PropertyFeatureViewModel> act = null)
        {
            if (entity == null)
                return;

            Entity = entity;
            act?.Invoke(this);
        }

        public string Value => Entity?.Value;

        public PropertyViewModel Property { get; set; }

        public FeatureViewModel Feature { get; set; }

        public override string ToString()
        {
            return Entity.ToString();
        }
    }
}