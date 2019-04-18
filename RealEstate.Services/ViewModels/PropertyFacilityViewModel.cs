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
        private readonly PropertyFacility _entity;

        public PropertyFacilityViewModel(PropertyFacility entity, bool includeDeleted, Action<PropertyFacilityViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            _entity = entity;
            Id = entity.Id;
            Logs = entity.GetLogs();
            action?.Invoke(this);
        }

        public void GetProperty(bool includeDeleted = false, Action<PropertyViewModel> action = null)
        {
            Property = _entity?.Property.Into(includeDeleted, action);
        }

        public void GetFacility(bool includeDeleted = false, Action<FacilityViewModel> action = null)
        {
            Facility = _entity?.Facility.Into(includeDeleted, action);
        }

        public PropertyViewModel Property { get; private set; }
        public FacilityViewModel Facility { get; private set; }
    }
}