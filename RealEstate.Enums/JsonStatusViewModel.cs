using Newtonsoft.Json;
using RealEstate.Base.Enums;

namespace RealEstate.Base
{
    public class JsonStatusViewModel
    {
        [JsonProperty("sts")]
        public int StatusCode { get; set; }

        [JsonProperty("msg")]
        public string Message { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }
}