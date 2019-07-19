using Microsoft.AspNetCore.Mvc;
using RealEstate.Resources;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.ViewModels
{
    public class TransApplicantViewModel
    {
        [HiddenInput]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public string ApplicantId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        [Display(ResourceType = typeof(SharedResource), Name = "To2")]
        public string NewUserId { get; set; }
    }
}