using Newtonsoft.Json;

namespace RealEstate.ViewModels.Json
{
    public class OwnerJsonViewModel
    {
        [JsonProperty("id")]
        public string OwnerId { get; set; }

        [JsonProperty("own")]
        public string OwnerName { get; set; }
    }
}