using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Base;
using RealEstate.Base.Attributes;
using RealEstate.Base.Enums;
using RealEstate.Resources;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.ViewModels.Input
{
    public class PaymentInputViewModel
    {
        [Display(ResourceType = typeof(SharedResource), Name = "Price")]
        [ValueValidation(RegexPatterns.NumbersOnly)]
        [Minimum(10000)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public double Value { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "For")]
        [ValueValidation(RegexPatterns.SafeText)]
        public string Text { get; set; }

        [HiddenInput]
        [Display(ResourceType = typeof(SharedResource), Name = "Employee")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public string EmployeeId { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "As")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public PaymentTypeEnum Type { get; set; }

        [FileTypeValidation("jpg", "png")]
        [Display(ResourceType = typeof(SharedResource), Name = "Picture")]
        public IFormFile[] Pictures { get; set; }
    }
}