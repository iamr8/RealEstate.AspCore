using Newtonsoft.Json;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels.Json
{
    public class SyncJsonViewModel
    {
        [JsonProperty("sts")]
        public int StatusCode { get; set; }

        [JsonProperty("msg")]
        public string Message { get; set; }

        [JsonProperty("itc")]
        public List<string> ItemCategories { get; set; }

        [JsonProperty("prc")]
        public List<string> PropertyCategories { get; set; }

        [JsonProperty("itm")]
        public List<ItemOutJsonViewModel> Items { get; set; }
    }
}