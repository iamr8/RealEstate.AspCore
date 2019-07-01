using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels.Json
{
    public class ItemOutJsonViewModel
    {
        [JsonProperty("prop")]
        public PropertyOutJsonViewModel Property { get; set; }

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

        [JsonProperty("l")]
        public LogViewModel Log { get; set; }
    }
}