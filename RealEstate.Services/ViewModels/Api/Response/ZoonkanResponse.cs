using Newtonsoft.Json;

namespace RealEstate.Services.ViewModels.Api.Response
{
    public class ZoonkanResponse
    {
        [JsonProperty("pc")]
        public string PropertyCategory { get; set; }

        [JsonProperty("ic")]
        public string ItemCategory { get; set; }

        [JsonProperty("c")]
        public int Count { get; set; }

        [JsonProperty("p")]
        public string Picture { get; set; }
    }
}