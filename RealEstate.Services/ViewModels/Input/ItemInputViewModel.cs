using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RealEstate.Base;
using RealEstate.Resources;
using RealEstate.Services.ViewModels.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.ViewModels.Input
{
    public class ItemInputViewModel : BaseInputViewModel
    {
        [Display(ResourceType = typeof(SharedResource), Name = "Property")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        [HiddenInput]
        public string PropertyId { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "DealCategory")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public string CategoryId { get; set; }

        [HiddenInput]
        public string ItemFeaturesJson { get; private set; }

        public List<FeatureJsonValueViewModel> ItemFeatures
        {
            get => ItemFeaturesJson.JsonGetAccessor<List<FeatureJsonValueViewModel>>();
            set => ItemFeaturesJson = value.JsonSetAccessor();
        }

        [Display(ResourceType = typeof(SharedResource), Name = "Description")]
        public string Description { get; set; }
    }
}