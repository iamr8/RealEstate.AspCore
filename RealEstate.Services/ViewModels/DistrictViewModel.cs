using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels
{
    public class DistrictViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public District Entity { get; }

        public DistrictViewModel(District entity, bool includeDeleted, Action<DistrictViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Entity = entity;
            action?.Invoke(this);
        }

        public string Name => Entity.Name;

        public void GetProperties(bool includeDeleted, Action<PropertyViewModel> action = null)
        {
            Properties = Entity?.Properties.Into(includeDeleted, action);
        }

        public List<PropertyViewModel> Properties { get; private set; }
    }
}