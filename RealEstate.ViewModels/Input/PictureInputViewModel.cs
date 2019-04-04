using Microsoft.AspNetCore.Http;
using RealEstate.Base;
using RealEstate.Extensions.Attributes;
using RealEstate.Resources;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.ViewModels.Input
{
    public class PictureInputViewModel : BaseInputViewModel
    {
        [R8AllowedFileTypes("jpg", "png")]
        [Display(ResourceType = typeof(SharedResource), Name = "UploadPhotos")]
        public IFormFile File { get; set; }

        public string Text { get; set; }
    }
}