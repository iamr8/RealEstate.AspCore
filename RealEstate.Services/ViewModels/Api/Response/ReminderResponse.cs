using Newtonsoft.Json;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels.Api.Response
{
    public class ReminderResponse
    {
        [JsonProperty("subj")]
        public string Subject { get; set; }

        [JsonProperty("dt")]
        public long Date { get; set; }

        [JsonProperty("cb")]
        public string CheckBank { get; set; }

        [JsonProperty("cn")]
        public string CheckNumber { get; set; }

        [JsonProperty("pr")]
        public long Price { get; set; }

        [JsonProperty("pics")]
        public List<string> Pictures { get; set; }
    }
}