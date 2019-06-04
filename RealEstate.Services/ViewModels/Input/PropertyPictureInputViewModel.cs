using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RealEstate.Base.Attributes;
using RealEstate.Resources;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.ViewModels.Input
{
    public class PropertyPictureInputViewModel
    {
        [FileTypeValidation("jpg", "png", "jpeg")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FileRequired")]
        [Display(ResourceType = typeof(SharedResource), Name = "Picture")]
        public IFormFile[] Pictures { get; set; }

        [HiddenInput]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public string PropertyId { get; set; }
    }
}