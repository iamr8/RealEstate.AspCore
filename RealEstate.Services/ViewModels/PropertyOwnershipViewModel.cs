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
        private readonly PropertyOwnership _entity;

        public PropertyOwnershipViewModel(PropertyOwnership entity, bool includeDeleted, Action<PropertyOwnershipViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            _entity = entity;
            Id = entity.Id;
            Logs = entity.GetLogs();
            action?.Invoke(this);
        }

        public void GetOwnerships(bool includeDeleted = false, Action<OwnershipViewModel> action = null)
        {
            Ownerships = _entity?.Ownerships.Into(includeDeleted, action);
        }

        public void GetProperty(bool includeDeleted = false, Action<PropertyViewModel> action = null)
        {
            Property = _entity?.Property.Into(includeDeleted, action);
        }

        public PropertyViewModel Property { get; private set; }
        public List<OwnershipViewModel> Ownerships { get; private set; }
    }
}