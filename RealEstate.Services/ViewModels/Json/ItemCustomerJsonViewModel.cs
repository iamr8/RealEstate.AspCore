using Newtonsoft.Json;

namespace RealEstate.Services.ViewModels.Json
{
    public class ItemCustomerJsonViewModel
    {
        [JsonProperty("id")]
        public string CustomerId { get; set; }

        [JsonProperty("api")]
        public string ApplicantId { get; set; }

        [JsonProperty("own")]
        public string Name { get; set; }

        [JsonProperty("mob")]
        public string Mobile { get; set; }
    }
}