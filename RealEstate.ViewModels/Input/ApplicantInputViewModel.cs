using Newtonsoft.Json;
using RealEstate.Base.Enums;
using RealEstate.ViewModels.Json;
using System.Collections.Generic;

namespace RealEstate.ViewModels.Input
{
    public class ApplicantInputViewModel : ContactViewModel
    {
        public ApplicantTypeEnum Type { get; set; }

        public string ApplicantFeaturesJson { get; set; }

        public List<FeatureJsonValueViewModel> ApplicantFeatures =>
            string.IsNullOrEmpty(ApplicantFeaturesJson)
                ? default
                : JsonConvert.DeserializeObject<List<FeatureJsonValueViewModel>>(ApplicantFeaturesJson);
    }
}