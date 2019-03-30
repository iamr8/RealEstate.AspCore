﻿using Newtonsoft.Json;

namespace RealEstate.ViewModels.Json
{
    public class FacilityJsonViewModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("nm")]
        public string Name { get; set; }
    }
}