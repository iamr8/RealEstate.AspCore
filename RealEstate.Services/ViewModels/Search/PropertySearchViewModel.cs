using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using RealEstate.Base;
using RealEstate.Resources;

namespace RealEstate.Services.ViewModels.Search
{
    public class PropertySearchViewModel : BaseSearchModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Address")]
        [JsonProperty("ad")]
        public string Address { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "District")]
        [JsonProperty("dis")]
        public string District { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "PropertyCategory")]
        [JsonProperty("cat")]
        public string Category { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Owner")]
        [JsonProperty("own")]
        public string Owner { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Owner")]
        [JsonProperty("owm")]
        public string OwnerMobile { get; set; }
    }
}