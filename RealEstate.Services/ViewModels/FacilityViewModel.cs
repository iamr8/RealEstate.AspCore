using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels
{
    public class FacilityViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public Facility Entity { get; }

        public FacilityViewModel(Facility entity, bool includeDeleted, Action<FacilityViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Entity = entity;
            action?.Invoke(this);
        }

        public string Name => Entity.Name;

        public void GetPropertyFacilities(bool includeDeleted, Action<PropertyFacilityViewModel> action = null)
        {
            PropertyFacilities = Entity?.PropertyFacilities.Into(includeDeleted, action);
        }

        public List<PropertyFacilityViewModel> PropertyFacilities { get; private set; }
    }
}