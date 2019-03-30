using RealEstate.Base;
using RealEstate.Resources;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.ViewModels.Input
{
    public class DistrictInputViewModel : BaseViewModel
    {
        [Display(ResourceType = typeof(SharedResource), Name = "Subject")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public string Name { get; set; }
    }
}