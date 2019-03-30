using Newtonsoft.Json;

namespace RealEstate.ViewModels.Json
{
    public class FacilityValueJsonViewModel
    {
        [JsonProperty("k")]
        public string Id { get; set; }

        [JsonProperty("n")]
        public string Name { get; set; }
    }
}