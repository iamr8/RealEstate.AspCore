using Newtonsoft.Json;

namespace RealEstate.Services.ViewModels.Json
{
    public class DistrictJsonViewModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("nm")]
        public string Name { get; set; }
    }
}