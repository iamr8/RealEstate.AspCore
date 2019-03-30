using Newtonsoft.Json;

namespace RealEstate.ViewModels.Json
{
    public class FeatureJsonValueViewModel : FeatureJsonViewModel
    {
        [JsonProperty("v")]
        public string Value { get; set; }
    }
}