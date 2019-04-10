using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using System.Collections.Generic;
using System.Linq;

namespace RealEstate.Services.ViewModels
{
    public class PropertyViewModel : BaseLogViewModel<Property>
    {
        public PropertyViewModel(Property model) : base(model)
        {
            if (model == null)
                return;

            Street = model.Street;
            Id = model.Id;
            Alley = model.Alley;
            BuildingName = model.BuildingName;
            Number = model.Number;
            Floor = model.Floor;
            Flat = model.Flat;
            Deals = model.Items.Sum(x => x.ItemRequests.Count(c => c.Deal?.Id != null));
            Description = model.Description;
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
        public List<PropertyOwnershipViewModel> Ownerships { get; set; }
        public List<PictureViewModel> Pictures { get; set; }
        public List<FacilityViewModel> Facilities { get; set; }
        public List<FeatureValueViewModel> Features { get; set; }
    }
}