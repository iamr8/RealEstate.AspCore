using Newtonsoft.Json;
using RealEstate.Base;

namespace RealEstate.Services.ViewModels.Json
{
    public class FacilityJsonViewModel : BaseViewModel
    {
        [JsonProperty("n")]
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}