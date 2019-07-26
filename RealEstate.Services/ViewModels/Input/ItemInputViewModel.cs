using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RealEstate.Base;
using RealEstate.Resources;
using RealEstate.Services.ViewModels.Json;

namespace RealEstate.Services.ViewModels.Input
{
    public class ItemInputViewModel : BaseInputViewModel
    {
        private string _itemFeaturesJson;
        private bool _isNegotiable;

        public PropertyInputViewModel Property { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "ItemCategory")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldRequired")]
        public string CategoryId { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Negotitable")]
        public bool IsNegotiable
        {
            get
            {
                const string negotiable = "قابل مذاکره";
                if (_isNegotiable)
                {
                    if (string.IsNullOrEmpty(Description))
                        Description = negotiable;
                    else
                    {
                        if (!Description.Contains(negotiable, StringComparison.CurrentCultureIgnoreCase))
                            Description += $" {negotiable}";
                    }
                }
                else
                    Description = Description?.Replace(negotiable, "", StringComparison.CurrentCultureIgnoreCase);
                return _isNegotiable;
            }
            set => _isNegotiable = value;
        }

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