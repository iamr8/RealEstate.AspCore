using RealEstate.Base;
using System.Collections.Generic;

namespace RealEstate.ViewModels
{
    public class PropertyViewModel : BaseLogViewModel
    {
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