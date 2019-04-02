using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RealEstate.Base;
using RealEstate.Resources;
using RealEstate.ViewModels.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.ViewModels.Input
{
    public class ItemRequestInputViewModel : BaseViewModel
    {
        [HiddenInput]
        [Display(ResourceType = typeof(SharedResource), Name = "Deal")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public string ItemId { get; set; }

        public string Description { get; set; }

        public string ApplicantsJson { get; set; }

        public List<ApplicantJsonViewModel> Applicants =>
            string.IsNullOrEmpty(ApplicantsJson)
                ? default
                : JsonConvert.DeserializeObject<List<ApplicantJsonViewModel>>(ApplicantsJson);
    }
}