using System.Collections.Generic;
using RealEstate.Base;

namespace RealEstate.ViewModels
{
    public class PropertyViewModel : BaseTrackViewModel
    {
        public string Address { get; set; }

        public int Deals { get; set; }
        public string Description { get; set; }
        public PropertyCategoryViewModel Category { get; set; }
        public GeolocationViewModel Geolocation { get; set; }

        public DistrictViewModel District { get; set; }
        public List<OwnerViewModel> Owners { get; set; }
        public List<PictureViewModel> Pictures { get; set; }
        public List<FacilityViewModel> Facilities { get; set; }
        public List<FeatureValueViewModel> Features { get; set; }
    }
}