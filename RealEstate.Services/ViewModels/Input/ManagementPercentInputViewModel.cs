using Newtonsoft.Json;
using RealEstate.Base;
using RealEstate.Resources;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.ViewModels.Input
{
    public class ManagementPercentInputViewModel : BaseInputViewModel
    {
        [Display(ResourceType = typeof(SharedResource), Name = "Employee")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public string EmployeeId { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Percent")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public int Percent { get; set; }
    }
}