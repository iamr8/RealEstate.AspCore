using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using RealEstate.Base;
using RealEstate.Base.Attributes;
using RealEstate.Resources;

namespace RealEstate.Services.ViewModels.Input
{
    public class PictureInputViewModel : BaseInputViewModel
    {
        [R8AllowedFileTypes("jpg", "png")]
        [Display(ResourceType = typeof(SharedResource), Name = "UploadPhotos")]
        public IFormFile File { get; set; }

        public string Text { get; set; }
    }
}