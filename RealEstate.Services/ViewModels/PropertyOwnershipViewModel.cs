using JetBrains.Annotations;
using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels
{
    public class PropertyOwnershipViewModel : BaseLogViewModel<PropertyOwnership>
    {
        [JsonIgnore]
        public PropertyOwnership Entity { get; set; }

        [CanBeNull]
        public readonly PropertyOwnershipViewModel Instance;

        public PropertyOwnershipViewModel(PropertyOwnership entity, bool includeDeleted) : base(entity)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Instance = new PropertyOwnershipViewModel
            {
                Entity = entity,
                Id = entity.Id,
                DateTime = entity.DateTime,
                Logs = entity.GetLogs()
            };
        }

        public PropertyOwnershipViewModel()
        {
        }

        public DateTime DateTime { get; set; }
        public List<OwnershipViewModel> Ownerships { get; set; }
        public PropertyViewModel Property { get; set; }
    }
}