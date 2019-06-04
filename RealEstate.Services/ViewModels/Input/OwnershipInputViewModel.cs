using Newtonsoft.Json;
using RealEstate.Resources;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.ViewModels.Input
{
    public class OwnershipInputViewModel : CustomerInputViewModel
    {
        [Display(ResourceType = typeof(SharedResource), Name = "Dong")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        [Range(1, 6, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "RangeError")]
        [DefaultValue(6)]
        [JsonProperty("dng")]
        public int Dong { get; set; }
    }
}