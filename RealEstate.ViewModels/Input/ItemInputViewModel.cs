using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RealEstate.Base;
using RealEstate.Resources;
using RealEstate.ViewModels.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.ViewModels.Input
{
    public class ItemInputViewModel : BaseViewModel
    {
        [Display(ResourceType = typeof(SharedResource), Name = "Property")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        [HiddenInput]
        public string PropertyId { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "DealCategory")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public string CategoryId { get; set; }

        [HiddenInput]
        public string ItemFeaturesJson { get; set; }

        public List<FeatureValueJsonViewModel> ItemFeatures =>
            string.IsNullOrEmpty(ItemFeaturesJson)
                ? default
                : JsonConvert.DeserializeObject<List<FeatureValueJsonViewModel>>(ItemFeaturesJson);

        [Display(ResourceType = typeof(SharedResource), Name = "Description")]
        public string Description { get; set; }
    }
}