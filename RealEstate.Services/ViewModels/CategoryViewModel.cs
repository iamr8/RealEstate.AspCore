using Newtonsoft.Json;
using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels
{
    public class CategoryViewModel : BaseLogViewModel<Category>
    {
        [JsonIgnore]
        private readonly Category _entity;

        public CategoryViewModel(Category entity, bool includeDeleted, Action<CategoryViewModel> action = null) : base(entity)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            _entity = entity;
            Id = entity.Id;
            Logs = entity.GetLogs();
            action?.Invoke(this);
        }

        public string Name => _entity.Name;

        public CategoryTypeEnum Type => _entity.Type;

        public void GetItems(bool includeDeleted, Action<ItemViewModel> action = null)
        {
            Items = _entity?.Items.Into(includeDeleted, action);
        }

        public void GetProperties(bool includeDeleted, Action<PropertyViewModel> action = null)
        {
            Properties = _entity?.Properties.Into(includeDeleted, action);
        }

        public void GetUserItemCategories(bool includeDeleted, Action<UserItemCategoryViewModel> action = null)
        {
            UserItemCategories = _entity?.UserItemCategories.Into(includeDeleted, action);
        }

        public void GetUserPropertyCategories(bool includeDeleted, Action<UserPropertyCategoryViewModel> action = null)
        {
            UserPropertyCategories = _entity?.UserPropertyCategories.Into(includeDeleted, action);
        }

        public List<ItemViewModel> Items { get; private set; }
        public List<PropertyViewModel> Properties { get; private set; }
        public List<UserItemCategoryViewModel> UserItemCategories { get; private set; }
        public List<UserPropertyCategoryViewModel> UserPropertyCategories { get; private set; }
    }
}