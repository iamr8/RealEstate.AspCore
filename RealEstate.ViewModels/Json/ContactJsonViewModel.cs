using Newtonsoft.Json;

namespace RealEstate.ViewModels.Json
{
    public class ContactJsonViewModel
    {
        [JsonProperty("ad")]
        public string Address { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("desc")]
        public string Description { get; set; }

        [JsonProperty("nm")]
        public string Name { get; set; }

        [JsonProperty("mob")]
        public string Mobile { get; set; }

        [JsonProperty("phn")]
        public string Phone { get; set; }
    }
}