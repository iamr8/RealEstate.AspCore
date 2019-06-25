using Newtonsoft.Json;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels.Json
{
    public class PropertyOutJsonViewModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("ad")]
        public string Address { get; set; }

        [JsonProperty("cat")]
        public string Category { get; set; }

        [JsonProperty("dis")]
        public string District { get; set; }

        [JsonProperty("own")]
        public OwnershipOutJsonViewModel Ownership { get; set; }

        [JsonProperty("ftr")]
        public Dictionary<string, string> Features { get; set; }

        [JsonProperty("fcl")]
        public List<string> Facilities { get; set; }

        [JsonProperty("pic")]
        public List<string> Pictures { get; set; }
    }
}