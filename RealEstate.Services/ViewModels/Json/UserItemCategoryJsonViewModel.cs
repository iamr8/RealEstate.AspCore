using Newtonsoft.Json;

namespace RealEstate.Services.ViewModels.Json
{
    public class UserItemCategoryJsonViewModel
    {
        [JsonProperty("n")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public string CategoryId { get; set; }
    }
}