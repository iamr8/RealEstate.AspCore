using Newtonsoft.Json;

namespace RealEstate.Services.ViewModels.Json
{
    public class DivisionJsonViewModel
    {
        [JsonProperty("n")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public string DivisionId { get; set; }
    }
}