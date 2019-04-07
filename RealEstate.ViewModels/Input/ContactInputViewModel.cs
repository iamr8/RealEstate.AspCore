using Newtonsoft.Json;
using RealEstate.Base;
using RealEstate.Resources;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.ViewModels.Input
{
    public class ContactInputViewModel : BaseInputViewModel
    {
        [Display(ResourceType = typeof(SharedResource), Name = "Mobile")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        [JsonProperty("mob")]
        public string Mobile { get; set; }
    }
}