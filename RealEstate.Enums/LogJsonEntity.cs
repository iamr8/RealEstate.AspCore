using Newtonsoft.Json;
using RealEstate.Base.Enums;
using System;

namespace RealEstate.Base
{
    public class LogJsonEntity
    {
        [JsonProperty("n")]
        public string Name { get; set; }

        [JsonProperty("m")]
        public string Mobile { get; set; }

        [JsonProperty("d")]
        public DateTime DateTime { get; set; }

        [JsonProperty("t")]
        public LogTypeEnum Type { get; set; }
    }
}