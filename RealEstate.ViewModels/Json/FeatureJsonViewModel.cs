using Newtonsoft.Json;
using RealEstate.Base;

namespace RealEstate.ViewModels.Json
{
    public class FeatureJsonViewModel : BaseViewModel
    {
        [JsonProperty("nm")]
        public string Name { get; set; }
    }
}