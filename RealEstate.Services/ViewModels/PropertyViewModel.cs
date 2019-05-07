using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RealEstate.Services.ViewModels
{
    public class PropertyViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public Property Entity { get; }

        public PropertyViewModel(Property entity, bool includeDeleted, Action<PropertyViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Entity = entity;
            action?.Invoke(this);
        }

        public string Address => Entity?.Address;
        public string Street => Entity?.Street;

        public string Alley => Entity?.Alley;

        public string BuildingName => Entity?.BuildingName;
        public string Number => Entity?.Number;
        public int Floor => Entity?.Floor ?? 0;
        public int Flat => Entity?.Flat ?? 0;

        public string Description => Entity.Description;

        public void GetCategory(bool includeDeleted = false, Action<CategoryViewModel> action = null)
        {
            Category = Entity?.Category.Into(includeDeleted, action);
        }

        public void GetItems(bool includeDeleted = false, Action<ItemViewModel> action = null)
        {
            Items = Entity?.Items.Into(includeDeleted, action);
        }

        public void GetDistrict(bool includeDeleted = false, Action<DistrictViewModel> action = null)
        {
            District = Entity?.District.Into(includeDeleted, action);
        }

        public void GetPropertyOwnerships(bool includeDeleted = false, Action<PropertyOwnershipViewModel> action = null)
        {
            PropertyOwnerships = Entity?.PropertyOwnerships.Into(includeDeleted, action);
        }

        public void GetPropertyFeatures(bool includeDeleted = false, Action<PropertyFeatureViewModel> action = null)
        {
            PropertyFeatures = Entity?.PropertyFeatures.Into(includeDeleted, action);
        }

        public void GetPropertyFacilities(bool includeDeleted = false, Action<PropertyFacilityViewModel> action = null)
        {
            PropertyFacilities = Entity?.PropertyFacilities.Into(includeDeleted, action);
        }

        public void GetPictures(bool includeDeleted = false, Action<PictureViewModel> action = null)
        {
            Pictures = Entity?.Pictures.Into(includeDeleted, action);
        }

        public CategoryViewModel Category { get; private set; }

        public GeolocationViewModel Geolocation => new GeolocationViewModel(Entity.Geolocation);

        public List<ItemViewModel> Items { get; private set; }

        public DistrictViewModel District { get; private set; }
        public List<PropertyOwnershipViewModel> PropertyOwnerships { get; private set; }
        public PropertyOwnershipViewModel CurrentPropertyOwnership => PropertyOwnerships?.OrderDescendingByCreationDateTime().FirstOrDefault();
        public List<PictureViewModel> Pictures { get; private set; }
        public List<PropertyFacilityViewModel> PropertyFacilities { get; private set; }
        public List<PropertyFeatureViewModel> PropertyFeatures { get; private set; }
    }
}