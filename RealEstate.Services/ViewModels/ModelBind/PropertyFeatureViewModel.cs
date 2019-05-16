using Newtonsoft.Json;
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

        public PropertyFeatureViewModel(PropertyFeature entity)
        {
            if (entity == null)
                return;

            Entity = entity;
        }

        public string Value => Entity?.Value;

        public Lazy<PropertyViewModel> Property =>
            LazyLoadExtension.LazyLoad(() => Entity?.Property.Map<Property, PropertyViewModel>());

        public Lazy<FeatureViewModel> Feature =>
            LazyLoadExtension.LazyLoad(() => Entity?.Feature.Map<Feature, FeatureViewModel>());
    }
}