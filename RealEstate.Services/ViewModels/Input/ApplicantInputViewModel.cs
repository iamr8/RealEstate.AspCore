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
    public class ApplicantInputViewModel : ContactInputViewModel
    {
        [Required]
        [Display(ResourceType = typeof(SharedResource), Name = "Type")]
        public ApplicantTypeEnum Type { get; set; }

        [HiddenInput]
        [Required]
        public string ApplicantFeaturesJson { get; private set; }

        public List<FeatureJsonValueViewModel> ApplicantFeatures
        {
            get => ApplicantFeaturesJson.JsonGetAccessor<List<FeatureJsonValueViewModel>>();
            set => ApplicantFeaturesJson = value.JsonSetAccessor();
        }
    }
}