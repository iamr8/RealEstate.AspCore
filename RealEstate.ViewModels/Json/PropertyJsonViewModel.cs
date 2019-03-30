﻿using System.Collections.Generic;
using Newtonsoft.Json;

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
        public List<FeatureValueJsonViewModel> Features { get; set; }

        [JsonProperty("psb")]
        public List<FacilityValueJsonViewModel> Facilities { get; set; }
    }
}