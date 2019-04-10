using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RealEstate.Base;
using RealEstate.Resources;
using RealEstate.Services.ViewModels.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.ViewModels.Input
{
    public class ItemRequestInputViewModel : BaseInputViewModel
    {
        [HiddenInput]
        [Display(ResourceType = typeof(SharedResource), Name = "Deal")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public string ItemId { get; set; }

        public string Description { get; set; }

        public string ApplicantsJson { get; set; }

        public List<ApplicantJsonViewModel> Applicants
        {
            get => string.IsNullOrEmpty(ApplicantsJson)
                ? default
                : JsonConvert.DeserializeObject<List<ApplicantJsonViewModel>>(ApplicantsJson);
            set => ApplicantsJson = JsonConvert.SerializeObject(value);
        }
    }
}