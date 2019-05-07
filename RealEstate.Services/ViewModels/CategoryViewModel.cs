using Newtonsoft.Json;
using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels
{
    public class CategoryViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public Category Entity { get; }

        public CategoryViewModel(Category entity, bool includeDeleted, Action<CategoryViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Entity = entity;
            action?.Invoke(this);
        }

        public string Name => Entity?.Name;

        public CategoryTypeEnum Type => Entity?.Type ?? CategoryTypeEnum.Property;

        public void GetItems(bool includeDeleted, Action<ItemViewModel> action = null)
        {
            Items = Entity?.Items.Into(includeDeleted, action).ShowBasedOn(x => x.Property);
        }

        public void GetProperties(bool includeDeleted, Action<PropertyViewModel> action = null)
        {
            Properties = Entity?.Properties.Into(includeDeleted, action);
        }

        public void GetUserItemCategories(bool includeDeleted, Action<UserItemCategoryViewModel> action = null)
        {
            UserItemCategories = Entity?.UserItemCategories.Into(includeDeleted, action);
        }

        public void GetUserPropertyCategories(bool includeDeleted, Action<UserPropertyCategoryViewModel> action = null)
        {
            UserPropertyCategories = Entity?.UserPropertyCategories.Into(includeDeleted, action);
        }

        public List<ItemViewModel> Items { get; private set; }
        public List<PropertyViewModel> Properties { get; private set; }
        public List<UserItemCategoryViewModel> UserItemCategories { get; private set; }
        public List<UserPropertyCategoryViewModel> UserPropertyCategories { get; private set; }
    }
}