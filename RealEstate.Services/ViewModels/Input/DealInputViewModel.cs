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
        private string _beneficiaryJson;
        private string _checksJson;
        public string Barcode { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "CommissionPrice")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public decimal Commission { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "TipPrice")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public decimal Tip { get; set; }

        [HiddenInput]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public string ItemId { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Description")]
        public string Description { get; set; }

        [HiddenInput]
        [Required]
        public string ChecksJson
        {
            get => _checksJson;
            set => _checksJson = value.JsonSetAccessor();
        }

        public List<CheckJsonViewModel> Checks
        {
            get => ChecksJson.JsonGetAccessor<List<CheckJsonViewModel>>();
            set => ChecksJson = value.JsonSetAccessor();
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