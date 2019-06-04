using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using System;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class PropertyFacilityViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public PropertyFacility Entity { get; }

        public PropertyFacilityViewModel(PropertyFacility entity, Action<PropertyFacilityViewModel> act = null)
        {
            if (entity == null)
                return;

            Entity = entity;
            act?.Invoke(this);
        }

        public PropertyViewModel Property { get; set; }

        public FacilityViewModel Facility { get; set; }

        public override string ToString()
        {
            return Entity.ToString();
        }
    }
}