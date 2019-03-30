using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Resources;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.ViewModels.Input
{
    public class UserInputViewModel : BaseViewModel
    {
        [Display(ResourceType = typeof(SharedResource), Name = "Username")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public string Username { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "FirstName")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public string FirstName { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "LastName")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public string LastName { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Mobile")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public string Mobile { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Role")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public Role Role { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Password")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public string Password { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Address")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public string Address { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "PhoneNumber")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public string Phone { get; set; }
    }
}