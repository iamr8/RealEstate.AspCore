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
        [ValueValidation(RegexPatterns.PersianText)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public string FirstName { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "LastName")]
        [ValueValidation(RegexPatterns.PersianText)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public string LastName { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Mobile")]
        [ValueValidation(RegexPatterns.Mobile)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public string Mobile { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Address")]
        [ValueValidation(RegexPatterns.SafeText)]
        public string Address { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "PhoneNumber")]
        [ValueValidation(RegexPatterns.NumbersOnly)]
        public string Phone { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "FixedSalary")]
        public double? FixedSalary { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Insurance")]
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