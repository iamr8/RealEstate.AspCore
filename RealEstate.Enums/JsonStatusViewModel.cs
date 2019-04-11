using Newtonsoft.Json;
using RealEstate.Base.Enums;

namespace RealEstate.Base
{
    public class JsonStatusViewModel
    {
        [JsonProperty("sts")]
        public StatusEnum Status { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }

    public class JsonStatusValueViewModel : JsonStatusViewModel
    {
        [JsonProperty("nm")]
        public string Name { get; set; }
    }
}