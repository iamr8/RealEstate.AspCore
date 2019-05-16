using RealEstate.Base;
using RealEstate.Base.Attributes;
using RealEstate.Base.Enums;
using RealEstate.Resources;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.ViewModels.Input
{
    public class PresenceInputViewModel : BaseInputViewModel
    {
        [Display(ResourceType = typeof(SharedResource), Name = "Type")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public PresenseStatusEnum Status { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Date")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        [ValueValidation(RegexPatterns.IranDate)]
        public string Date { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Time")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        [ValueValidation(RegexPatterns.Time)]
        public string Time { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Employee")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public string EmployeeId { get; set; }
    }
}