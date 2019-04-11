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
        public string DealPaymentsJson { get; private set; }

        public List<DealPaymentJsonViewModel> DealPayments
        {
            get => DealPaymentsJson.JsonGetAccessor<List<DealPaymentJsonViewModel>>();
            set => DealPaymentsJson = value.JsonSetAccessor();
        }

        [HiddenInput]
        [Required]
        public string BeneficiaryJson { get; private set; }

        public List<BeneficiaryJsonViewModel> Beneficiaries
        {
            get => BeneficiaryJson.JsonGetAccessor<List<BeneficiaryJsonViewModel>>();
            set => BeneficiaryJson = value.JsonSetAccessor();
        }
    }
}