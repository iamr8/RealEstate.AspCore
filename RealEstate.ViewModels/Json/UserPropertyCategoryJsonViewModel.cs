using Newtonsoft.Json;
using RealEstate.Base;

namespace RealEstate.ViewModels.Json
{
    public class UserPropertyCategoryJsonViewModel : BaseViewModel
    {
        [JsonProperty("n")]
        public string Name { get; set; }
    }
}