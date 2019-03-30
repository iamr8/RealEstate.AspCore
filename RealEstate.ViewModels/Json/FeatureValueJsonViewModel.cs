using Newtonsoft.Json;

namespace RealEstate.ViewModels.Json
{
    public class FeatureValueJsonViewModel
    {
        [JsonProperty("k")]
        public string Id { get; set; }

        [JsonProperty("n")]
        public string Name { get; set; }

        [JsonProperty("v")]
        public string Value { get; set; }
    }
}