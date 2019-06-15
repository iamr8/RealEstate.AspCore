using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RealEstate.Base;
using RealEstate.Resources;
using RealEstate.Services.ViewModels.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.ViewModels.Input
{
    public class ItemComplexInputViewModel : BaseInputViewModel
    {
        private string _itemFeaturesJson;

        public PropertyComplexInputViewModel Property { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "ItemCategory")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public string CategoryId { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Negotitable")]
        public bool IsNegotiable { get; set; }

        [HiddenInput]
        public string ItemFeaturesJson
        {
            get => _itemFeaturesJson;
            set => _itemFeaturesJson = value.JsonSetAccessor();
        }

        public List<FeatureJsonValueViewModel> ItemFeatures
        {
            get => ItemFeaturesJson.JsonGetAccessor<FeatureJsonValueViewModel>();
            set => ItemFeaturesJson = value.JsonSetAccessor();
        }

        [Display(ResourceType = typeof(SharedResource), Name = "Description")]
        public string Description { get; set; }
    }
}