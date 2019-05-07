using Newtonsoft.Json;
using RealEstate.Base.Enums;
using System;
using System.Collections.Generic;

namespace RealEstate.Base
{
    public class LogJsonEntity
    {
        [JsonProperty("i")]
        public string UserId { get; set; }

        [JsonProperty("n")]
        public string UserFullName { get; set; }

        [JsonProperty("m")]
        public string UserMobile { get; set; }

        [JsonProperty("d")]
        public DateTime DateTime { get; set; }

        [JsonProperty("t")]
        public LogTypeEnum Type { get; set; }

        [JsonProperty("f")]
        public List<LogModifyDetailJsonEntity> Modifies { get; set; }
    }

    public class LogModifyDetailJsonEntity
    {
        [JsonProperty("k")]
        public string Key { get; set; }

        [JsonProperty("v")]
        public string Value { get; set; }
    }
}