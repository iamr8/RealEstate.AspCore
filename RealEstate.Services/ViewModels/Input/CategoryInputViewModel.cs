using System.ComponentModel.DataAnnotations;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Resources;

namespace RealEstate.Services.ViewModels.Input
{
    public class CategoryInputViewModel : BaseInputViewModel
    {
        [Display(ResourceType = typeof(SharedResource), Name = "Subject")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "FeatureType")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public CategoryTypeEnum Type { get; set; }
    }
}