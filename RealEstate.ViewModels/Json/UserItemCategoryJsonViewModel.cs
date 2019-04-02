using Newtonsoft.Json;
using RealEstate.Base;

namespace RealEstate.ViewModels.Json
{
    public class UserItemCategoryJsonViewModel : BaseViewModel
    {
        [JsonProperty("n")]
        public string Name { get; set; }
    }
}