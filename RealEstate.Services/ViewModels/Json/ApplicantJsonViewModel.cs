using Newtonsoft.Json;

namespace RealEstate.Services.ViewModels.Json
{
    public class ApplicantJsonViewModel
    {
        [JsonProperty("id")]
        public string ContactId { get; set; }

        [JsonProperty("api")]
        public string ApplicantId { get; set; }

        [JsonProperty("own")]
        public string Name { get; set; }

        [JsonProperty("mob")]
        public string Mobile { get; set; }
    }
}