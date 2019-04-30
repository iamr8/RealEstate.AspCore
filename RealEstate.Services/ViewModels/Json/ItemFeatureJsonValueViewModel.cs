using Newtonsoft.Json;
using RealEstate.Base;

namespace RealEstate.Services.ViewModels.Json
{
    public class ItemFeatureJsonValueViewModel : FeatureJsonViewModel
    {
        [JsonProperty("f")]
        public string From { get; set; }

        [JsonProperty("t")]
        public string To { get; set; }
    }
}