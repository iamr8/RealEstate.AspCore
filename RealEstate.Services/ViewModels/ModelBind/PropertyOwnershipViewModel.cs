using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using System;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class PropertyOwnershipViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public PropertyOwnership Entity { get; }

        public PropertyOwnershipViewModel(PropertyOwnership entity, Action<PropertyOwnershipViewModel> act = null)
        {
            if (entity == null)
                return;

            Entity = entity;
            act?.Invoke(this);
        }

        public PropertyViewModel Property { get; set; }

        public List<OwnershipViewModel> Ownerships { get; set; }

        public override string ToString()
        {
            return Entity?.ToString();
        }
    }
}