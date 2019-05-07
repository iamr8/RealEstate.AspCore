using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;

namespace RealEstate.Services.ViewModels
{
    public class PropertyFacilityViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public PropertyFacility Entity { get; }

        public PropertyFacilityViewModel(PropertyFacility entity, bool includeDeleted, Action<PropertyFacilityViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Entity = entity;
            action?.Invoke(this);
        }

        public void GetProperty(bool includeDeleted = false, Action<PropertyViewModel> action = null)
        {
            Property = Entity?.Property.Into(includeDeleted, action);
        }

        public void GetFacility(bool includeDeleted = false, Action<FacilityViewModel> action = null)
        {
            Facility = Entity?.Facility.Into(includeDeleted, action);
        }

        public PropertyViewModel Property { get; private set; }
        public FacilityViewModel Facility { get; private set; }
    }
}