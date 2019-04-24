using Newtonsoft.Json;
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
        public List<(string, string)> ItemFeatures { get; set; }
    }
}