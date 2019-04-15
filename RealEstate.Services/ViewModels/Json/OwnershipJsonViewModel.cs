using Newtonsoft.Json;

namespace RealEstate.Services.ViewModels.Json
{
    public class OwnershipJsonViewModel
    {
        [JsonProperty("dng")]
        public int Dong { get; set; }

        [JsonProperty("id")]
        public string ContactId { get; set; }

        [JsonProperty("own")]
        public string Name { get; set; }

        [JsonProperty("mob")]
        public string Mobile { get; set; }
    }
}