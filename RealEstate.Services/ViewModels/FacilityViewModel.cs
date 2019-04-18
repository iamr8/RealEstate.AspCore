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
        private readonly Facility _entity;

        public FacilityViewModel(Facility entity, bool includeDeleted, Action<FacilityViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            _entity = entity;
            Id = entity.Id;
            Logs = entity.GetLogs();
            action?.Invoke(this);
        }

        public string Name => _entity.Name;

        public void GetPropertyFacilities(bool includeDeleted, Action<PropertyFacilityViewModel> action = null)
        {
            PropertyFacilities = _entity?.PropertyFacilities.Into(includeDeleted, action);
        }

        public List<PropertyFacilityViewModel> PropertyFacilities { get; private set; }
    }
}