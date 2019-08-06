using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RealEstate.Base;
using RealEstate.Base.Attributes;
using RealEstate.Resources;
using RealEstate.Services.ViewModels.Json;

namespace RealEstate.Services.ViewModels.Input
{
    public class PropertyInputViewModel : BaseInputViewModel
    {
        private string _propertyFeaturesJson;
        private string _propertyFacilitiesJson;

        [Display(ResourceType = typeof(SharedResource), Name = "Address")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        [ValueValidation(RegexPatterns.SafeText)]
        public string Address { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "BuildingName")]
        [ValueValidation(RegexPatterns.SafeText)]
        public string BuildingName { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "BuildingNumber")]
        [ValueValidation(RegexPatterns.SafeText)]
        public string Number { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Floor")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        [DefaultValue(0)]
        public int Floor { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "FlatNumber")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        [DefaultValue(0)]
        public int Flat { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "PropertyCategory")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public string CategoryId { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "District")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public string DistrictId { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public OwnershipInputViewModel Ownership { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "PropertyFeatures")]
        [HiddenInput]
        public string PropertyFeaturesJson
        {
            get => _propertyFeaturesJson;
            set => _propertyFeaturesJson = JsonExtensions.InitJson(value);
        }

        public List<FeatureJsonValueViewModel> PropertyFeatures
        {
            get => JsonExtensions.Deserialize<List<FeatureJsonValueViewModel>>(PropertyFeaturesJson);
            set => PropertyFeaturesJson = value.Serialize();
        }

        [Display(ResourceType = typeof(SharedResource), Name = "PropertyFacilities")]
        [HiddenInput]
        public string PropertyFacilitiesJson
        {
            get => _propertyFacilitiesJson;
            set => _propertyFacilitiesJson = JsonExtensions.InitJson(value);
        }

        public List<FacilityJsonViewModel> PropertyFacilities
        {
            get => JsonExtensions.Deserialize<List<FacilityJsonViewModel>>(PropertyFacilitiesJson);
            set => PropertyFacilitiesJson = value.Serialize();
        }
    }
}