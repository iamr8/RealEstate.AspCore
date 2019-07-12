using Newtonsoft.Json;
using RealEstate.Base;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
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

        private FeatureFixExtension.NormalizeFeatureStatus Normalized => Feature != null
            ? Entity?.Value.FixPersian().NormalizeFeature(Feature.Name)
            : new FeatureFixExtension.NormalizeFeatureStatus(Entity?.Value.FixPersian(), Entity?.Value.FixPersian());

        public string Value => Normalized.Value;
        public string OriginalValue => Normalized.OriginalValue;
        public PropertyViewModel Property { get; set; }

        public FeatureViewModel Feature { get; set; }

        public override string ToString()
        {
            return Entity.ToString();
        }
    }
}