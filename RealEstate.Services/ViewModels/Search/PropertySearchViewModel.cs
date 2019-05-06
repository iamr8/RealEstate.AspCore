using RealEstate.Base;
using RealEstate.Base.Attributes;
using RealEstate.Resources;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.ViewModels.Search
{
    public class PropertySearchViewModel : BaseSearchModel
    {
        [SearchParameter("propertyId")]
        public string Id { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Street")]
        [SearchParameter("propertyStreet")]
        public string Street { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "District")]
        [SearchParameter("propertyDistrict")]
        public string District { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "PropertyCategory")]
        [SearchParameter("propertyCategory")]
        public string Category { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Owner")]
        [SearchParameter("propertyOwner")]
        public string Owner { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Mobile")]
        [SearchParameter("propertyOwnerMobile")]
        public string OwnerMobile { get; set; }
    }
}