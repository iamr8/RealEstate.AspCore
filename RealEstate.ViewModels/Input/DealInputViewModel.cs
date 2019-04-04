using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RealEstate.Base;
using RealEstate.Resources;
using RealEstate.ViewModels.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.ViewModels.Input
{
    public class DealInputViewModel : BaseInputViewModel
    {
        [Display(ResourceType = typeof(SharedResource), Name = "Description")]
        public string Description { get; set; }

        [HiddenInput]
        [Required]
        public string DealPaymentsJson { get; set; }

        [Required]
        public List<DealPaymentJsonViewModel> DealPayments =>
            string.IsNullOrEmpty(DealPaymentsJson)
                ? default
                : JsonConvert.DeserializeObject<List<DealPaymentJsonViewModel>>(DealPaymentsJson);

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