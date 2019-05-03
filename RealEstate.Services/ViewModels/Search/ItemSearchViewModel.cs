using Microsoft.AspNetCore.Mvc;
using RealEstate.Base;
using RealEstate.Base.Attributes;
using RealEstate.Resources;
using RealEstate.Services.ViewModels.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace RealEstate.Services.ViewModels.Search
{
    public class ItemSearchViewModel : BaseSearchModel
    {
        private string _featuresJson;
        private string _facilitiesJson;

        //public bool IsUnderCondition => IncludeDeletedItems
        //                                || !string.IsNullOrEmpty(Street)
        //                                || !string.IsNullOrEmpty(ItemCategory)
        //                                || !string.IsNullOrEmpty(PropertyCategory)
        //                                || !string.IsNullOrEmpty(OwnerMobile)
        //                                || !string.IsNullOrEmpty(Owner)
        //                                || !string.IsNullOrEmpty(District)
        //                                || Features?.Any() == true
        //                                || Facilities?.Any() == true;

        [Display(ResourceType = typeof(SharedResource), Name = "Street")]
        [SearchParameter("street")]
        public string Street { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "ShowDeletedItems")]
        [SearchParameter("deleted")]
        public bool IncludeDeletedItems { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "ItemCategory")]
        [SearchParameter("itemCategory")]
        public string ItemCategory { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "OwnerMobile")]
        [SearchParameter("ownerMobile")]
        public string OwnerMobile { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "PropertyCategory")]
        [SearchParameter("propertyCategory")]
        public string PropertyCategory { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Owner")]
        [SearchParameter("ownerName")]
        public string Owner { get; set; }

        [SearchParameter("id")]
        public string ItemId { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "District")]
        [SearchParameter("district")]
        public string District { get; set; }

        [SearchParameter("customerId")]
        public string CustomerId { get; set; }

        [HiddenInput]
        [SearchParameter("features", typeof(ItemFeatureJsonValueViewModel))]
        public string FeaturesJson
        {
            get => _featuresJson.DecodeJson<ItemFeatureJsonValueViewModel>();
            set => _featuresJson = value.JsonSetAccessor();
        }

        public List<ItemFeatureJsonValueViewModel> Features
        {
            get => FeaturesJson.JsonGetAccessor<ItemFeatureJsonValueViewModel>().AddNoneToLast();
            set => FeaturesJson = value.JsonSetAccessor();
        }

        [HiddenInput]
        [SearchParameter("facilities", typeof(ItemFacilityJsonViewModel))]
        public string FacilitiesJson
        {
            get => _facilitiesJson.DecodeJson<ItemFacilityJsonViewModel>();
            set => _facilitiesJson = value.JsonSetAccessor();
        }

        public List<ItemFacilityJsonViewModel> Facilities
        {
            get => FacilitiesJson.JsonGetAccessor<ItemFacilityJsonViewModel>().AddNoneToLast();
            set => FacilitiesJson = value.JsonSetAccessor();
        }

        //[Display(ResourceType = typeof(SharedResource), Name = "Feature")]
        //[SearchParameter("itemFeature")]
        //public string FeatureName { get; set; }

        //[Display(ResourceType = typeof(SharedResource), Name = "From")]
        //[SearchParameter("itemFeatureFrom")]
        //public double? FromValue { get; set; }

        //[Display(ResourceType = typeof(SharedResource), Name = "To")]
        //[SearchParameter("itemFeatureTo")]
        //public double? ToValue { get; set; }
    }
}