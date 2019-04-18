﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RealEstate.Base;
using RealEstate.Resources;
using RealEstate.Services.ViewModels.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.ViewModels.Input
{
    public class ItemRequestInputViewModel
    {
        [HiddenInput]
        [Display(ResourceType = typeof(SharedResource), Name = "Deal")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public string ItemId { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Description")]
        public string Description { get; set; }

        [HiddenInput]
        [Required]
        public string CustomerJson { get; set; }

        public List<ApplicantJsonViewModel> Customers
        {
            get => CustomerJson.JsonGetAccessor<List<ApplicantJsonViewModel>>();
            set => CustomerJson = value.JsonSetAccessor();
        }
    }
}