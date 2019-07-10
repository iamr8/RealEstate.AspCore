using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels.Api.Response
{
    public class ItemResponse
    {
        [JsonProperty("prop")]
        public PropertyResponse Property { get; set; }

        [JsonProperty("cat")]
        public string Category { get; set; }

        [JsonProperty("ftr")]
        public Dictionary<string, string> Features { get; set; }

        [JsonProperty("neg")]
        public bool IsNegotiable { get; set; }

        [JsonProperty("desc")]
        public string Description { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonIgnore]
        [JsonProperty("l")]
        public LogViewModel Log { get; set; }
    }

    public class PropertyResponse
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
        public OwnershipResponse Ownership { get; set; }

        [JsonProperty("ftr")]
        public Dictionary<string, string> Features { get; set; }

        [JsonProperty("fcl")]
        public List<string> Facilities { get; set; }

        [JsonProperty("pic")]
        public List<string> Pictures { get; set; }
    }

    public class OwnershipResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("own")]
        public string Name { get; set; }

        [JsonProperty("owm")]
        public string Mobile { get; set; }
    }
}