using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RealEstate.Base;
using RealEstate.Resources;
using RealEstate.ViewModels.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.ViewModels.Input
{
    public class DealInputViewModel : BaseViewModel
    {
        [HiddenInput]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public string DealId { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "TipPrice")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public decimal TipPrice { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "CommissionPrice")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public decimal CommissionPrice { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Description")]
        public string Description { get; set; }

        [HiddenInput]
        [Required]
        public string BeneficiaryJson { get; set; }

        [Required]
        public List<BeneficiaryJsonViewModel> Beneficiaries =>
            string.IsNullOrEmpty(BeneficiaryJson)
                ? default
                : JsonConvert.DeserializeObject<List<BeneficiaryJsonViewModel>>(BeneficiaryJson);
    }
}