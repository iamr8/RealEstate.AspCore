using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RealEstate.Base.Enums;
using RealEstate.Resources;
using RealEstate.ViewModels.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.ViewModels.Input
{
    public class ApplicantInputViewModel : ContactInputViewModel
    {
        [Required]
        [Display(ResourceType = typeof(SharedResource), Name = "Type")]
        public ApplicantTypeEnum Type { get; set; }

        [HiddenInput]
        [Required]
        public string ApplicantFeaturesJson { get; set; }

        public List<FeatureJsonValueViewModel> ApplicantFeatures =>
            string.IsNullOrEmpty(ApplicantFeaturesJson)
                ? default
                : JsonConvert.DeserializeObject<List<FeatureJsonValueViewModel>>(ApplicantFeaturesJson);
    }
}