using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class PropertyFacilityViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public PropertyFacility Entity { get; }

        public PropertyFacilityViewModel(PropertyFacility entity)
        {
            if (entity == null)
                return;

            Entity = entity;
        }

        public Lazy<PropertyViewModel> Property =>
            LazyLoadExtension.LazyLoad(() => Entity?.Property.Map<Property, PropertyViewModel>());

        public Lazy<FacilityViewModel> Facility =>
            LazyLoadExtension.LazyLoad(() => Entity?.Facility.Map<Facility, FacilityViewModel>());
    }
}