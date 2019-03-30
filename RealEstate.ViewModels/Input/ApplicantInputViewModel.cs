using Newtonsoft.Json;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Extensions;
using RealEstate.Resources;
using RealEstate.ViewModels.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.ViewModels.Input
{
    public class ApplicantInputViewModel : BaseViewModel
    {
        public ApplicantTypeEnum Type { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "FirstName")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        [JsonProperty("nm")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "PhoneNumber")]
        [JsonProperty("phn")]
        public string Phone { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Mobile")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        [JsonProperty("mob")]
        public string Mobile { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Address")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        [R8Validator(RegexPatterns.SafeText)]
        [JsonProperty("ad")]
        public string Address { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Description")]
        [JsonProperty("desc")]
        public string Description { get; set; }

        public string ApplicantFeaturesJson { get; set; }

        public List<FeatureJsonValueViewModel> ApplicantFeatures =>
            string.IsNullOrEmpty(ApplicantFeaturesJson)
                ? default
                : JsonConvert.DeserializeObject<List<FeatureJsonValueViewModel>>(ApplicantFeaturesJson);
    }
}