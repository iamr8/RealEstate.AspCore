using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RealEstate.Base;
using RealEstate.Resources;
using RealEstate.Services.ViewModels.Json;

namespace RealEstate.Services.ViewModels.Input
{
    public class PropertyInputViewModel : BaseInputViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public string Street { get; set; }

        public string Alley { get; set; }

        public string BuildingName { get; set; }
        public string Number { get; set; }
        public int Floor { get; set; }
        public int Flat { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "PropertyCategory")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public string CategoryId { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Description")]
        public string Description { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "District")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public string DistrictId { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        [HiddenInput]
        public string OwnershipsJson { get; set; }

        [Required]
        public List<OwnershipJsonViewModel> Ownerships =>
            string.IsNullOrEmpty(OwnershipsJson)
                ? default
                : JsonConvert.DeserializeObject<List<OwnershipJsonViewModel>>(OwnershipsJson);

        [HiddenInput]
        public string PropertyFeaturesJson { get; set; }

        [Required]
        public List<FeatureJsonValueViewModel> PropertyFeatures =>
            string.IsNullOrEmpty(PropertyFeaturesJson)
                ? default
                : JsonConvert.DeserializeObject<List<FeatureJsonValueViewModel>>(PropertyFeaturesJson);

        [HiddenInput]
        public string PropertyFacilitiesJson { get; set; }

        [Required]
        public List<Json.FacilityJsonViewModel> PropertyFacilities =>
            string.IsNullOrEmpty(PropertyFacilitiesJson)
                ? default
                : JsonConvert.DeserializeObject<List<Json.FacilityJsonViewModel>>(PropertyFacilitiesJson);
    }
}