using Newtonsoft.Json;

namespace RealEstate.Services.ViewModels.Json
{
    public class OwnershipOutJsonViewModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("own")]
        public string Name { get; set; }

        [JsonProperty("owm")]
        public string Mobile { get; set; }
    }
}