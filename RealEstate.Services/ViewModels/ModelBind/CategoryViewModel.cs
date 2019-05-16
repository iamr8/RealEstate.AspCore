using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class CategoryViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public Category Entity { get; }

        public CategoryViewModel(Category entity)
        {
            if (entity == null)
                return;

            Entity = entity;
        }

        public string Name => Entity?.Name;

        public CategoryTypeEnum Type => Entity?.Type ?? CategoryTypeEnum.Property;

        public Lazy<List<ItemViewModel>> Items => LazyLoadExtension.LazyLoad(() => Entity?.Items.Map<Item, ItemViewModel>());
        public Lazy<List<PropertyViewModel>> Properties => LazyLoadExtension.LazyLoad(() => Entity?.Properties.Map<Property, PropertyViewModel>());
        public Lazy<List<UserItemCategoryViewModel>> UserItemCategories => LazyLoadExtension.LazyLoad(() => Entity?.UserItemCategories.Map<UserItemCategory, UserItemCategoryViewModel>());
        public Lazy<List<UserPropertyCategoryViewModel>> UserPropertyCategories => LazyLoadExtension.LazyLoad(() => Entity?.UserPropertyCategories.Map<UserPropertyCategory, UserPropertyCategoryViewModel>());
    }
}