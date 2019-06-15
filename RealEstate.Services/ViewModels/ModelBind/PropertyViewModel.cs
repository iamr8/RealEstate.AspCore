using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class PropertyViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public Property Entity { get; }

        public PropertyViewModel(Property entity, Action<PropertyViewModel> act = null)
        {
            if (entity == null)
                return;

            Entity = entity;
            act?.Invoke(this);
        }

        public string Address => Entity?.Address;
        public string AddressHtmlStyled => Entity?.AddressHtmlStyled;
        public string Street => Entity?.Street;

        public string Alley => Entity?.Alley;

        public string BuildingName => Entity?.BuildingName;
        public string Number => Entity?.Number;
        public int Floor => Entity?.Floor ?? 0;
        public int Flat => Entity?.Flat ?? 0;
        public GeolocationViewModel Geolocation => Entity != null ? new GeolocationViewModel(Entity.Geolocation) : default;

        public string Description => Entity?.Description;

        public PropertyOwnershipViewModel CurrentPropertyOwnership => PropertyOwnerships?.OrderDescendingByCreationDateTime().FirstOrDefault();

        public CategoryViewModel Category { get; set; }

        public DistrictViewModel District { get; set; }

        public List<ItemViewModel> Items { get; set; }

        public List<PropertyOwnershipViewModel> PropertyOwnerships { get; set; }

        public List<PictureViewModel> Pictures { get; set; }

        public List<PropertyFacilityViewModel> PropertyFacilities { get; set; }

        public List<PropertyFeatureViewModel> PropertyFeatures { get; set; }

        public override string ToString()
        {
            return Entity?.ToString();
        }
    }
}