using RealEstate.Base;
using RealEstate.Base.Attributes;
using RealEstate.Resources;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.ViewModels.Input
{
    public class LeaveInputViewModel : BaseInputViewModel
    {
        [Display(ResourceType = typeof(SharedResource), Name = "FromDate")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        [ValueValidation(RegexPatterns.IranDate)]
        public string FromDate { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "FromHour")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        [Range(0, 23)]
        public int FromHour { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "ToDate")]
        [ValueValidation(RegexPatterns.IranDate)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public string ToDate { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "FromHour")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        [Range(0, 23)]
        public int ToHour { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Reason")]
        public string Reason { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Employee")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public string EmployeeId { get; set; }
    }
}