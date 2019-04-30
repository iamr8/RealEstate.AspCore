using Newtonsoft.Json;
using RealEstate.Base;

namespace RealEstate.Services.ViewModels.Json
{
    public class ItemFacilityJsonViewModel
    {
        [JsonProperty("n")]
        public string Name { get; set; }
    }
}