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

        [Display(ResourceType = typeof(SharedResource), Name = "Address")]
        [SearchParameter("propertyAddress")]
        public string Address { get; set; }

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

        [Display(ResourceType = typeof(SharedResource), Name = "Category")]
        [SearchParameter("propertyCategory")]
        public string CategoryName { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Feature")]
        [SearchParameter("propertyFeature")]
        public string FeatureName { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "From")]
        [SearchParameter("propertyFeatureFrom")]
        public double? FromValue { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "To")]
        [SearchParameter("propertyFeatureTo")]
        public double? ToValue { get; set; }
    }
}