using Newtonsoft.Json;
using RealEstate.Base.Attributes;
using RealEstate.Resources;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.ViewModels.Json
{
    public class ItemFacilityJsonViewModel
    {
        [Display(ResourceType = typeof(SharedResource), Name = "FirstName")]
        [Order(0)]
        [JsonProperty("n")]
        public string Name { get; set; }
    }
}