using Newtonsoft.Json;

namespace RealEstate.ViewModels.Json
{
    public class OwnershipJsonViewModel
    {
        [JsonProperty("id")]
        public string OwnershipId { get; set; }

        [JsonProperty("own")]
        public string ContactName { get; set; }
    }
}