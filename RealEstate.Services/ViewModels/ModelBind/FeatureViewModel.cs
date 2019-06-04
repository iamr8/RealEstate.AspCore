using Newtonsoft.Json;
using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using System;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class FeatureViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public Feature Entity { get; }

        public FeatureViewModel(Feature entity, Action<FeatureViewModel> act = null)
        {
            if (entity == null)
                return;

            Entity = entity;
            act?.Invoke(this);
        }

        public string Name => Entity?.Name;

        public FeatureTypeEnum Type => Entity?.Type ?? FeatureTypeEnum.Property;

        public List<PropertyFeatureViewModel> PropertyFeatures { get; set; }

        public override string ToString()
        {
            return Entity.ToString();
        }
    }
}