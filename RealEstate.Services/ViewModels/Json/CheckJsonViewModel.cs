using Newtonsoft.Json;

namespace RealEstate.Services.ViewModels.Json
{
    public class CheckJsonViewModel
    {
        [JsonProperty("dt")]
        public string Date { get; set; }

        [JsonProperty("bnk")]
        public string Bank { get; set; }

        [JsonProperty("nmb")]
        public string Number { get; set; }

        [JsonProperty("prc")]
        public decimal Price { get; set; }
    }
}