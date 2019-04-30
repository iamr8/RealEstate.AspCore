using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RealEstate.Base;
using RealEstate.Resources;
using RealEstate.Services.ViewModels.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
            set => _customerJson = value.JsonSetAccessor();
        }

        public List<ItemCustomerJsonViewModel> Customers
        {
            get => CustomerJson.JsonGetAccessor<ItemCustomerJsonViewModel>();
            set => CustomerJson = value.JsonSetAccessor();
        }
    }
}