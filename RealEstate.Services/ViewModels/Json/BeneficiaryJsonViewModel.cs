using Newtonsoft.Json;
using RealEstate.Base;

namespace RealEstate.Services.ViewModels.Json
{
    public class BeneficiaryJsonViewModel
    {
        [JsonProperty("k")]
        public string UserId { get; set; }

        [JsonProperty("nm")]
        public string UserFullName { get; set; }

        [JsonProperty("tip")]
        public int TipPercent { get; set; }

        [JsonProperty("cms")]
        public int CommissionPercent { get; set; }
    }
}