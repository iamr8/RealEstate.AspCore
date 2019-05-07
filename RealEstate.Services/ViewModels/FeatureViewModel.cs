using Newtonsoft.Json;
using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels
{
    public class FeatureViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public Feature Entity { get; }

        public FeatureViewModel(Feature entity, bool includeDeleted, Action<FeatureViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Entity = entity;
            action?.Invoke(this);
        }

        public string Name => Entity.Name;

        public FeatureTypeEnum Type => Entity.Type;

        public void GetPropertyFeatures(bool includeDeleted, Action<PropertyFeatureViewModel> action = null)
        {
            PropertyFeatures = Entity?.PropertyFeatures.Into(includeDeleted, action);
        }

        public List<PropertyFeatureViewModel> PropertyFeatures { get; private set; }
    }
}