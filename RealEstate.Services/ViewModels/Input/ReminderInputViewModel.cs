using Microsoft.AspNetCore.Http;
using RealEstate.Base;
using RealEstate.Base.Attributes;
using RealEstate.Resources;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.ViewModels.Input
{
    public class ReminderInputViewModel : BaseInputViewModel
    {
        [Display(ResourceType = typeof(SharedResource), Name = "Subject")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        [ValueValidation(RegexPatterns.SafeText)]
        public string Description { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "RememberDeadline")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        [ValueValidation(RegexPatterns.IranDate)]
        public string Date { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "CheckNumber")]
        public string CheckNumber { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "CheckBank")]
        public string CheckBank { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Price")]
        public decimal? Price { get; set; }

        [FileTypeValidation("jpg", "png", "jpeg")]
        [Display(ResourceType = typeof(SharedResource), Name = "Picture")]
        public IFormFile[] Pictures { get; set; }
    }
}