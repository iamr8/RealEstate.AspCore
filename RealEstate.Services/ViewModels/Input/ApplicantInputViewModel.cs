using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Resources;
using RealEstate.Services.ViewModels.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.ViewModels.Input
{
    public class ApplicantInputViewModel : CustomerInputViewModel
    {
        private string _applicantFeaturesJson;

        [Required]
        [Display(ResourceType = typeof(SharedResource), Name = "Type")]
        public ApplicantTypeEnum Type { get; set; }

        [HiddenInput]
        [Display(ResourceType = typeof(SharedResource), Name = "ApplicantFeatures")]
        public string ApplicantFeaturesJson
        {
            get => _applicantFeaturesJson;
            set => _applicantFeaturesJson = value.JsonSetAccessor();
        }

        public List<FeatureJsonValueViewModel> ApplicantFeatures
        {
            get => ApplicantFeaturesJson.JsonGetAccessor<FeatureJsonValueViewModel>();
            set => ApplicantFeaturesJson = value.JsonSetAccessor();
        }
    }
}