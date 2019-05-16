using Newtonsoft.Json;
using RealEstate.Base;
using RealEstate.Base.Attributes;
using RealEstate.Resources;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.ViewModels.Input
{
    public class CustomerInputViewModel : BaseInputViewModel
    {
        [Display(ResourceType = typeof(SharedResource), Name = "Mobile")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        [JsonProperty("mob")]
        [ValueValidation(RegexPatterns.Mobile)]
        public string Mobile { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "FirstName")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        [JsonProperty("nm")]
        [ValueValidation(RegexPatterns.PersianText)]
        public string Name { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "PhoneNumber")]
        [ValueValidation(RegexPatterns.NumbersOnly)]
        [JsonProperty("phn")]
        public string Phone { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Address")]
        [ValueValidation(RegexPatterns.SafeText)]
        [JsonProperty("ad")]
        public string Address { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Description")]
        [ValueValidation(RegexPatterns.SafeText)]
        [JsonProperty("desc")]
        public string Description { get; set; }
    }
}