using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RealEstate.Base;
using RealEstate.Resources;
using RealEstate.Services.ViewModels.Json;

namespace RealEstate.Services.ViewModels.Input
{
    public class DealRequestInputViewModel : BaseInputViewModel
    {
        private string _customerJson;

        [Display(ResourceType = typeof(SharedResource), Name = "Description")]
        public string Description { get; set; }

        [HiddenInput]
        [Required]
        public string CustomerJson
        {
            get => _customerJson;
            set => _customerJson = JsonExtensions.InitJson(value);
        }

        public List<ItemCustomerJsonViewModel> Customers
        {
            get => JsonExtensions.Deserialize<List<ItemCustomerJsonViewModel>>(CustomerJson);
            set => CustomerJson = value.Serialize();
        }
    }
}