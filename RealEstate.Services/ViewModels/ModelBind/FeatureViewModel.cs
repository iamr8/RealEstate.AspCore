using Newtonsoft.Json;
using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class FeatureViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public Feature Entity { get; }

        public FeatureViewModel(Feature entity)
        {
            if (entity == null)
                return;

            Entity = entity;
        }

        public string Name => Entity?.Name;

        public FeatureTypeEnum Type => Entity?.Type ?? FeatureTypeEnum.Property;

        public Lazy<List<PropertyFeatureViewModel>> PropertyFeatures =>
            LazyLoadExtension.LazyLoad(() => Entity?.PropertyFeatures.Into<PropertyFeature, PropertyFeatureViewModel>());
    }
}