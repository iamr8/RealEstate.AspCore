using Newtonsoft.Json;
using RealEstate.Base.Enums;

namespace RealEstate.Services.ViewModels.Json
{
    public class CategoryJsonViewModel
    {
        [JsonProperty("nm")]
        public string Name { get; set; }

        [JsonProperty("typ")]
        public CategoryTypeEnum Type { get; set; }

        [JsonProperty("id")]
        public string CategoryId { get; set; }
    }
}