using Newtonsoft.Json;
using RealEstate.Base;

namespace RealEstate.Services.ViewModels.Json
{
    public class ReminderJsonViewModel : BaseViewModel
    {
        [JsonProperty("tp")]
        public decimal Tip { get; set; }

        [JsonProperty("cms")]
        public decimal Commission { get; set; }

        [JsonProperty("pd")]
        public string Date { get; set; }

        [JsonProperty("chb")]
        public string CheckBank { get; set; }

        [JsonProperty("chn")]
        public string CheckNumber { get; set; }

        [JsonProperty("pr")]
        public decimal Price { get; set; }

        [JsonProperty("desc")]
        public string Description { get; set; }
    }
}