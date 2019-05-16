using Microsoft.AspNetCore.Mvc;
using RealEstate.Base;
using RealEstate.Base.Attributes;
using RealEstate.Resources;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.ViewModels.Input
{
    public class UserLoginViewModel
    {
        [Display(ResourceType = typeof(SharedResource), Name = "Username")]
        [ValueValidation(RegexPatterns.EnglishText)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        [MinLength(4, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "MinLength")]
        public string Username { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Password")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        [MinLength(6, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "MinLength")]
        public string Password { get; set; }

        [HiddenInput]
        public string ReturnUrl { get; set; }
    }
}