using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class FacilityViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public Facility Entity { get; }

        public FacilityViewModel(Facility entity)
        {
            if (entity == null)
                return;

            Entity = entity;
        }

        public string Name => Entity?.Name;

        public Lazy<List<PropertyFacilityViewModel>> PropertyFacilities =>
            LazyLoadExtension.LazyLoad(() => Entity?.PropertyFacilities.Into<PropertyFacility, PropertyFacilityViewModel>());
    }
}