using Newtonsoft.Json;

namespace RealEstate.Services.ViewModels.Api.Response
{
    public class FeatureResponse
    {
        [JsonProperty("n")]
        public string Name { get; set; }

        [JsonProperty("v")]
        public string Value { get; set; }
    }
}