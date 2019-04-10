using System.ComponentModel.DataAnnotations;
using RealEstate.Base;
using RealEstate.Resources;

namespace RealEstate.Services.ViewModels.Input
{
    public class DistrictInputViewModel : BaseInputViewModel
    {
        [Display(ResourceType = typeof(SharedResource), Name = "Subject")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public string Name { get; set; }
    }
}