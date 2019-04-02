using Newtonsoft.Json;
using RealEstate.Base;

namespace RealEstate.ViewModels.Json
{
    public class FacilityJsonViewModel : BaseViewModel
    {
        [JsonProperty("n")]
        public string Name { get; set; }
    }
}