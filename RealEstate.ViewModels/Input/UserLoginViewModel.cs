using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Resources;

namespace RealEstate.ViewModels.Input
{
    public class UserLoginViewModel
    {
        [Display(ResourceType = typeof(SharedResource), Name = "Username")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public string Username { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Password")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public string Password { get; set; }

        [HiddenInput]
        public string ReturnUrl { get; set; }
    }
}