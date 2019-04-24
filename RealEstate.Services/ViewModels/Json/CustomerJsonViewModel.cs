using Newtonsoft.Json;
using RealEstate.Base;

namespace RealEstate.Services.ViewModels.Json
{
    public class CustomerJsonViewModel : BaseViewModel
    {
        [JsonProperty("ad")]
        public string Address { get; set; }

        [JsonProperty("desc")]
        public string Description { get; set; }

        [JsonProperty("nm")]
        public string Name { get; set; }

        [JsonProperty("mob")]
        public string Mobile { get; set; }

        [JsonProperty("phn")]
        public string Phone { get; set; }
    }
}