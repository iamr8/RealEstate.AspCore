using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels
{
    public class PropertyOwnershipViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public PropertyOwnership Entity { get; }

        public PropertyOwnershipViewModel(PropertyOwnership entity, bool includeDeleted, Action<PropertyOwnershipViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Entity = entity;
            action?.Invoke(this);
        }

        public void GetOwnerships(bool includeDeleted = false, Action<OwnershipViewModel> action = null)
        {
            Ownerships = Entity?.Ownerships.Into(includeDeleted, action);
        }

        public void GetProperty(bool includeDeleted = false, Action<PropertyViewModel> action = null)
        {
            Property = Entity?.Property.Into(includeDeleted, action);
        }

        public PropertyViewModel Property { get; private set; }
        public List<OwnershipViewModel> Ownerships { get; private set; }
    }
}