using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
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
        private string _dealPaymentsJson;
        private string _beneficiaryJson;

        public string ItemId { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Description")]
        public string Description { get; set; }

        [HiddenInput]
        [Required]
        public string DealPaymentsJson
        {
            get => _dealPaymentsJson;
            set => _dealPaymentsJson = value.JsonSetAccessor();
        }

        public List<DealPaymentJsonViewModel> DealPayments
        {
            get => DealPaymentsJson.JsonGetAccessor<List<DealPaymentJsonViewModel>>();
            set => DealPaymentsJson = value.JsonSetAccessor();
        }

        [HiddenInput]
        [Required]
        public string BeneficiaryJson
        {
            get => _beneficiaryJson;
            set => _beneficiaryJson = value.JsonSetAccessor();
        }

        public List<BeneficiaryJsonViewModel> Beneficiaries
        {
            get => BeneficiaryJson.JsonGetAccessor<List<BeneficiaryJsonViewModel>>();
            set => BeneficiaryJson = value.JsonSetAccessor();
        }
    }
}