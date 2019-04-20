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
        private readonly Property _entity;

        public PropertyViewModel(Property entity, bool includeDeleted, Action<PropertyViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            _entity = entity;
            Id = entity.Id;
            Logs = entity.GetLogs();
            action?.Invoke(this);
        }

        public string Street => _entity?.Street;

        public string Alley => _entity?.Alley;

        public string BuildingName => _entity?.BuildingName;
        public string Number => _entity?.Number;
        public int Floor => _entity?.Floor ?? 0;
        public int Flat => _entity?.Flat ?? 0;

        public string Description => _entity.Description;

        public void GetCategory(bool includeDeleted = false, Action<CategoryViewModel> action = null)
        {
            Category = _entity?.Category.Into(includeDeleted, action);
        }

        public void GetItems(bool includeDeleted = false, Action<ItemViewModel> action = null)
        {
            Items = _entity?.Items.Into(includeDeleted, action);
        }

        public void GetDistrict(bool includeDeleted = false, Action<DistrictViewModel> action = null)
        {
            District = _entity?.District.Into(includeDeleted, action);
        }

        public void GetPropertyOwnerships(bool includeDeleted = false, Action<PropertyOwnershipViewModel> action = null)
        {
            PropertyOwnerships = _entity?.PropertyOwnerships.Into(includeDeleted, action);
        }

        public void GetPropertyFeatures(bool includeDeleted = false, Action<PropertyFeatureViewModel> action = null)
        {
            PropertyFeatures = _entity?.PropertyFeatures.Into(includeDeleted, action);
        }

        public void GetPropertyFacilities(bool includeDeleted = false, Action<PropertyFacilityViewModel> action = null)
        {
            PropertyFacilities = _entity?.PropertyFacilities.Into(includeDeleted, action);
        }

        public void GetPictures(bool includeDeleted = false, Action<PictureViewModel> action = null)
        {
            Pictures = _entity?.Pictures.Into(includeDeleted, action);
        }

        public CategoryViewModel Category { get; private set; }

        public GeolocationViewModel Geolocation => new GeolocationViewModel(_entity.Geolocation);

        public List<ItemViewModel> Items { get; private set; }

        public DistrictViewModel District { get; private set; }
        public List<PropertyOwnershipViewModel> PropertyOwnerships { get; private set; }
        public PropertyOwnershipViewModel CurrentPropertyOwnership => PropertyOwnerships?.OrderDescendingByCreationDateTime().FirstOrDefault();
        public List<PictureViewModel> Pictures { get; private set; }
        public List<PropertyFacilityViewModel> PropertyFacilities { get; private set; }
        public List<PropertyFeatureViewModel> PropertyFeatures { get; private set; }
    }
}