using Newtonsoft.Json;
using RealEstate.Base;
using RealEstate.Base.Attributes;
using RealEstate.Resources;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.ViewModels.Json
{
    public class FeatureJsonViewModel : BaseViewModel
    {
        [JsonProperty("nm")]
        [Display(ResourceType = typeof(SharedResource), Name = "FirstName")]
        [Order(0)]
        public string Name { get; set; }
    }
}