using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RealEstate.Base;
using RealEstate.Base.Attributes;
using RealEstate.Base.Enums;
using RealEstate.Resources;
using RealEstate.Services.ViewModels.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.ViewModels.Input
{
    public class UserInputViewModel : BaseInputViewModel
    {
        [Display(ResourceType = typeof(SharedResource), Name = "Username")]
        [R8Validator(RegexPatterns.EnglishText)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public string Username { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Password")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public string Password { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Role")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public Role Role { get; set; }

        [Required]
        public string EmployeeId { get; set; }

        [HiddenInput]
        public string UserPropertyCategoriesJson { get; private set; }

        public List<UserPropertyCategoryJsonViewModel> UserPropertyCategories
        {
            get => UserPropertyCategoriesJson.JsonGetAccessor<List<UserPropertyCategoryJsonViewModel>>();
            set => UserPropertyCategoriesJson = value.JsonSetAccessor();
        }

        [HiddenInput]
        public string UserItemCategoriesJson { get; private set; }

        public List<UserItemCategoryJsonViewModel> UserItemCategories
        {
            get => UserItemCategoriesJson.JsonGetAccessor<List<UserItemCategoryJsonViewModel>>();
            set => UserItemCategoriesJson = value.JsonSetAccessor();
        }
    }
}