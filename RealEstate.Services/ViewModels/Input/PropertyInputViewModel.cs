using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RealEstate.Base;
using RealEstate.Resources;
using RealEstate.Services.ViewModels.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

        public List<OwnershipJsonViewModel> Ownerships
        {
            get => string.IsNullOrEmpty(OwnershipsJson)
                ? default
                : JsonConvert.DeserializeObject<List<OwnershipJsonViewModel>>(OwnershipsJson);
            set => OwnershipsJson = JsonConvert.SerializeObject(value);
        }

        [HiddenInput]
        public string PropertyFeaturesJson { get; set; }

        public List<FeatureJsonValueViewModel> PropertyFeatures
        {
            get => string.IsNullOrEmpty(PropertyFeaturesJson)
                ? default
                : JsonConvert.DeserializeObject<List<FeatureJsonValueViewModel>>(PropertyFeaturesJson);
            set => PropertyFeaturesJson = JsonConvert.SerializeObject(value);
        }

        [HiddenInput]
        public string PropertyFacilitiesJson { get; set; }

        public List<FacilityJsonViewModel> PropertyFacilities
        {
            get => string.IsNullOrEmpty(PropertyFacilitiesJson)
                ? default
                : JsonConvert.DeserializeObject<List<Json.FacilityJsonViewModel>>(PropertyFacilitiesJson);
            set => PropertyFacilitiesJson = JsonConvert.SerializeObject(value);
        }
    }
}