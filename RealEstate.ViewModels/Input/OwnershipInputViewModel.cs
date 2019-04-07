using Newtonsoft.Json;
using RealEstate.Base;
using RealEstate.Extensions;
using RealEstate.Resources;
using System.ComponentModel.DataAnnotations;
using RealEstate.Extensions.Attributes;

namespace RealEstate.ViewModels.Input
{
    public class OwnershipInputViewModel : ContactInputViewModel
    {
        public int Dong { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "FirstName")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        [JsonProperty("nm")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "PhoneNumber")]
        [JsonProperty("phn")]
        public string Phone { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Address")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        [R8Validator(RegexPatterns.SafeText)]
        [JsonProperty("ad")]
        public string Address { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Description")]
        [JsonProperty("desc")]
        public string Description { get; set; }
    }
}