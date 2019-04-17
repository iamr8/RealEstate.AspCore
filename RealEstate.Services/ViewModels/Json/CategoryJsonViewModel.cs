using Newtonsoft.Json;
using RealEstate.Base;
using RealEstate.Base.Enums;

namespace RealEstate.Services.ViewModels.Json
{
    public class CategoryJsonViewModel : BaseViewModel
    {
        [JsonProperty("nm")]
        public string Name { get; set; }

        [JsonProperty("typ")]
        public CategoryTypeEnum Type { get; set; }
    }
}