using Newtonsoft.Json;
using RealEstate.Base.Enums;
using RealEstate.Domain.Tables;
using RealEstate.Services.BaseLog;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels
{
    public class CategoryViewModel : BaseLogViewModel<Category>
    {
        public CategoryViewModel(Category entity) : base(entity)
        {
            if (entity == null)
                return;

            Name = entity.Name;
            Type = entity.Type;
            Id = entity.Id;
        }

        public CategoryViewModel()
        {
        }

        [JsonProperty("nm")]
        public string Name { get; set; }

        [JsonProperty("typ")]
        public CategoryTypeEnum Type { get; set; }

        public List<ItemViewModel> Items { get; set; }
        public List<PropertyViewModel> Properties { get; set; }
    }
}