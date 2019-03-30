using Newtonsoft.Json;
using System.Collections.Generic;

namespace RealEstate.ViewModels.Json
{
    public class PropertyJsonViewModel
    {
        [JsonProperty("ad")]
        public string Address { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("cat")]
        public string Category { get; set; }

        [JsonProperty("desc")]
        public string Description { get; set; }

        [JsonProperty("dis")]
        public string District { get; set; }

        [JsonProperty("own")]
        public string Owner { get; set; }

        [JsonProperty("owm")]
        public string OwnerMobile { get; set; }

        [JsonProperty("ftr")]
        public List<FeatureJsonValueViewModel> Features { get; set; }

        [JsonProperty("psb")]
        public List<FacilityJsonViewModel> Facilities { get; set; }
    }
}