using Newtonsoft.Json;
using RealEstate.Base;

namespace RealEstate.ViewModels
{
    public class CategoryViewModel : BaseLogViewModel
    {
        [JsonProperty("nm")]
        public string Name { get; set; }
    }
}