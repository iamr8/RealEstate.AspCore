using Newtonsoft.Json;
using RealEstate.Base.Attributes;
using RealEstate.Resources;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.ViewModels.Json
{
    public class ItemFeatureJsonValueViewModel : FeatureJsonViewModel
    {
        [JsonProperty("f")]
        [Display(ResourceType = typeof(SharedResource), Name = "From")]
        [Order(0)]
        public string From { get; set; }

        [JsonProperty("t")]
        [Display(ResourceType = typeof(SharedResource), Name = "To")]
        [Order(1)]
        public string To { get; set; }
    }
}