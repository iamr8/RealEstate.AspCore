using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RealEstate.Base;
using RealEstate.Base.Attributes;
using RealEstate.Resources;
using RealEstate.Services.ViewModels.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.ViewModels.Search
{
    public class ItemSearchViewModel : BaseSearchModel
    {
        private string _featuresJson;
        private string _facilitiesJson;

        [Display(ResourceType = typeof(SharedResource), Name = "Street")]
        [SearchParameter("street")]
        public string Street { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "ItemCategory")]
        [SearchParameter("itemCategory")]
        public string ItemCategory { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "OwnerMobile")]
        [SearchParameter("ownerMobile")]
        public string OwnerMobile { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "PropertyCategory")]
        [SearchParameter("propertyCategory")]
        public string PropertyCategory { get; set; }

        [SearchParameter("hasFeature")]
        public string HasFeature { get; set; }

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
        [Display(ResourceType = typeof(SharedResource), Name = "Features")]
        [SearchParameter("features", typeof(ItemFeatureJsonValueViewModel))]
        [JsonIgnore]
        public string FeaturesJson
        {
            get => _featuresJson.DecodeJson<ItemFeatureJsonValueViewModel>();
            set => _featuresJson = value.JsonSetAccessor();
        }

        [Display(ResourceType = typeof(SharedResource), Name = "Features")]
        public List<ItemFeatureJsonValueViewModel> Features
        {
            get => FeaturesJson.JsonGetAccessor<ItemFeatureJsonValueViewModel>().AddNoneToLast();
            set => FeaturesJson = value.JsonSetAccessor();
        }

        [HiddenInput]
        [JsonIgnore]
        [Display(ResourceType = typeof(SharedResource), Name = "Facilities")]
        [SearchParameter("facilities", typeof(ItemFacilityJsonViewModel))]
        public string FacilitiesJson
        {
            get => _facilitiesJson.DecodeJson<ItemFacilityJsonViewModel>();
            set => _facilitiesJson = value.JsonSetAccessor();
        }

        [Display(ResourceType = typeof(SharedResource), Name = "Facilities")]
        public List<ItemFacilityJsonViewModel> Facilities
        {
            get => FacilitiesJson.JsonGetAccessor<ItemFacilityJsonViewModel>().AddNoneToLast();
            set => FacilitiesJson = value.JsonSetAccessor();
        }
    }
}