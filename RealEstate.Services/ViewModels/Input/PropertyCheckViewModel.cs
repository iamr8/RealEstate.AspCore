using RealEstate.Resources;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.ViewModels.Input
{
    public class PropertyCheckViewModel
    {
        [Display(ResourceType = typeof(SharedResource), Name = "District")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public string District { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Category")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public string Category { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Street")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public string Street { get; set; }
    }
}