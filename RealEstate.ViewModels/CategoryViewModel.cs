using Newtonsoft.Json;
using RealEstate.Base;
using RealEstate.Base.Enums;
using System.Collections.Generic;

namespace RealEstate.ViewModels
{
    public class CategoryViewModel : BaseLogViewModel
    {
        [JsonProperty("nm")]
        public string Name { get; set; }

        [JsonProperty("typ")]
        public CategoryTypeEnum Type { get; set; }

        public List<ItemViewModel> Items { get; set; }
        public List<PropertyViewModel> Properties { get; set; }
    }
}