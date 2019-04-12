using JetBrains.Annotations;
using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace RealEstate.Services.ViewModels
{
    public class PropertyViewModel : BaseLogViewModel<Property>
    {
        [JsonIgnore]
        public Property Entity { get; set; }

        [CanBeNull]
        public readonly PropertyViewModel Instance;

        public PropertyViewModel(Property entity, bool includeDeleted) : base(entity)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Instance = new PropertyViewModel
            {
                Entity = entity,
                Street = entity.Street,
                Id = entity.Id,
                Alley = entity.Alley,
                BuildingName = entity.BuildingName,
                Number = entity.Number,
                Floor = entity.Floor,
                Flat = entity.Flat,
                Deals = entity.Items?.Sum(x => x.ItemRequests.Count(c => c.Deal?.Id != null)) ?? 0,
                Description = entity.Description,
                Logs = entity.GetLogs()
            };
        }

        public PropertyViewModel()
        {
        }

        public string Street { get; set; }

        public string Alley { get; set; }

        public string BuildingName { get; set; }
        public string Number { get; set; }
        public int Floor { get; set; }
        public int Flat { get; set; }
        public int Deals { get; set; }
        public string Description { get; set; }
        public CategoryViewModel Category { get; set; }
        public GeolocationViewModel Geolocation { get; set; }

        public DistrictViewModel District { get; set; }
        public List<ItemViewModel> Items { get; set; }
        public List<PropertyOwnershipViewModel> PropertyOwnerships { get; set; }
        public PropertyOwnershipViewModel CurrentPropertyOwnership => PropertyOwnerships?.OrderByDescending(x => x.DateTime).FirstOrDefault();
        public List<PictureViewModel> Pictures { get; set; }
        public List<PropertyFacilityViewModel> Facilities { get; set; }
        public List<PropertyFeatureViewModel> Features { get; set; }
    }
}