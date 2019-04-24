using Newtonsoft.Json;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels.Json
{
    public class PropertyOutJsonViewModel
    {
        [JsonProperty("ad")]
        public string Address { get; set; }

        [JsonProperty("cat")]
        public string Category { get; set; }

        [JsonProperty("desc")]
        public string Description { get; set; }

        [JsonProperty("dis")]
        public string District { get; set; }

        [JsonProperty("own")]
        public List<string> Ownerships { get; set; }

        [JsonProperty("ftr")]
        public List<(string, string)> Features { get; set; }

        [JsonProperty("fcl")]
        public List<string> Facilities { get; set; }
    }
}