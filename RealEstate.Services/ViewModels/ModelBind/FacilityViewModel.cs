using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using System;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class FacilityViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public Facility Entity { get; }

        public FacilityViewModel(Facility entity, Action<FacilityViewModel> act = null)
        {
            if (entity == null)
                return;

            Entity = entity;
            act?.Invoke(this);
        }

        public string Name => Entity?.Name;

        public List<PropertyFacilityViewModel> PropertyFacilities { get; set; }

        public override string ToString()
        {
            return Entity.ToString();
        }
    }
}