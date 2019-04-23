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
        private string _remindersJson;
        private string _beneficiaryJson;
        public string Barcode { get; set; }
        public decimal Commission { get; set; }
        public decimal Tip { get; set; }
        public string ItemId { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Description")]
        public string Description { get; set; }

        [HiddenInput]
        [Required]
        public string RemindersJson
        {
            get => _remindersJson;
            set => _remindersJson = value.JsonSetAccessor();
        }

        public List<ReminderJsonViewModel> Reminders
        {
            get => RemindersJson.JsonGetAccessor<List<ReminderJsonViewModel>>();
            set => RemindersJson = value.JsonSetAccessor();
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