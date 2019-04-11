using Newtonsoft.Json;

namespace RealEstate.Services.ViewModels.Json
{
    public class OwnershipJsonViewModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("own")]
        public string Name { get; set; }

        [JsonProperty("mob")]
        public string Mobile { get; set; }

        [JsonProperty("dng")]
        public int Dong { get; set; }
    }
}