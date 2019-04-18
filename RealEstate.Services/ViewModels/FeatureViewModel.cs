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
        private readonly Feature _entity;

        public FeatureViewModel(Feature entity, bool includeDeleted, Action<FeatureViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            _entity = entity;
            Id = entity.Id;
            Logs = entity.GetLogs();
            action?.Invoke(this);
        }

        public string Name => _entity.Name;

        public FeatureTypeEnum Type => _entity.Type;

        public void GetPropertyFeatures(bool includeDeleted, Action<PropertyFeatureViewModel> action = null)
        {
            PropertyFeatures = _entity?.PropertyFeatures.Into(includeDeleted, action);
        }

        public List<PropertyFeatureViewModel> PropertyFeatures { get; private set; }
    }
}