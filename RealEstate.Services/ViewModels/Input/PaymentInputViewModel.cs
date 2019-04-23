using RealEstate.Base;
using RealEstate.Base.Attributes;
using RealEstate.Resources;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.ViewModels.Input
{
    public class PaymentInputViewModel : BaseInputViewModel
    {
        [Display(ResourceType = typeof(SharedResource), Name = "Value")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public double Value { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Description")]
        [R8Validator(RegexPatterns.SafeText)]
        public string Text { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Employee")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public string EmployeeId { get; set; }
    }
}