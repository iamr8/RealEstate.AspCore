using RealEstate.Base;
using RealEstate.Base.Attributes;
using RealEstate.Resources;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.ViewModels.Input
{
    public class ReminderInputViewModel : BaseInputViewModel
    {
        [Display(ResourceType = typeof(SharedResource), Name = "Description")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public string Description { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Date")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        [R8Validator(RegexPatterns.IranDate)]
        public string Date { get; set; }
    }
}