using Microsoft.AspNetCore.Mvc;
using RealEstate.Base;
using RealEstate.Base.Attributes;
using RealEstate.Resources;
using RealEstate.Services.ViewModels.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.ViewModels.Input
{
    public class EmployeeInputViewModel : BaseInputViewModel
    {
        private string _divisionsJson;

        [Display(ResourceType = typeof(SharedResource), Name = "FirstName")]
        [R8Validator(RegexPatterns.PersianText)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public string FirstName { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "LastName")]
        [R8Validator(RegexPatterns.PersianText)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public string LastName { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Mobile")]
        [R8Validator(RegexPatterns.Mobile)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public string Mobile { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Address")]
        [R8Validator(RegexPatterns.SafeText)]
        public string Address { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "PhoneNumber")]
        [R8Validator(RegexPatterns.NumbersOnly)]
        public string Phone { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "FixedSalary")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public double? FixedSalary { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Insurance")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public double? Insurance { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Divisions")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        [HiddenInput]
        public string DivisionsJson
        {
            get => _divisionsJson;
            set => _divisionsJson = value.JsonSetAccessor();
        }

        public List<DivisionJsonViewModel> Divisions
        {
            get => DivisionsJson.JsonGetAccessor<DivisionJsonViewModel>();
            set => DivisionsJson = value.JsonSetAccessor();
        }
    }
}