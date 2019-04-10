using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RealEstate.Base;
using RealEstate.Resources;
using RealEstate.Services.ViewModels.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.ViewModels.Input
{
    public class DealInputViewModel : BaseInputViewModel
    {
        [Display(ResourceType = typeof(SharedResource), Name = "Description")]
        public string Description { get; set; }

        [HiddenInput]
        [Required]
        public string DealPaymentsJson { get; set; }

        public List<DealPaymentJsonViewModel> DealPayments
        {
            get => string.IsNullOrEmpty(DealPaymentsJson)
                ? default
                : JsonConvert.DeserializeObject<List<DealPaymentJsonViewModel>>(DealPaymentsJson);
            set => DealPaymentsJson = JsonConvert.SerializeObject(value);
        }

        [HiddenInput]
        [Required]
        public string BeneficiaryJson { get; set; }

        public List<BeneficiaryJsonViewModel> Beneficiaries
        {
            get => string.IsNullOrEmpty(BeneficiaryJson)
                ? default
                : JsonConvert.DeserializeObject<List<BeneficiaryJsonViewModel>>(BeneficiaryJson);
            set => BeneficiaryJson = JsonConvert.SerializeObject(value);
        }
    }
}