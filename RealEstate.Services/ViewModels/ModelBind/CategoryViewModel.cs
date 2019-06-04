using Newtonsoft.Json;
using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using System;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class CategoryViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public Category Entity { get; }

        public CategoryViewModel(Category entity, Action<CategoryViewModel> act = null)
        {
            if (entity == null)
                return;

            Entity = entity;
            act?.Invoke(this);
        }

        public string Name => Entity?.Name;

        public CategoryTypeEnum Type => Entity?.Type ?? CategoryTypeEnum.Property;

        public List<ItemViewModel> Items { get; set; }
        public List<PropertyViewModel> Properties { get; set; }
        public List<UserItemCategoryViewModel> UserItemCategories { get; set; }
        public List<UserPropertyCategoryViewModel> UserPropertyCategories { get; set; }

        public override string ToString()
        {
            return Entity.ToString();
        }
    }
}