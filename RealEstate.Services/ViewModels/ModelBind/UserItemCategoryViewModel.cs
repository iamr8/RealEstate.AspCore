using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class UserItemCategoryViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public UserItemCategory Entity { get; }

        public UserItemCategoryViewModel(UserItemCategory entity)
        {
            if (entity == null)
                return;

            Entity = entity;
        }

        public Lazy<UserViewModel> User =>
            LazyLoadExtension.LazyLoad(() => Entity?.User.Map<User, UserViewModel>());

        public Lazy<CategoryViewModel> Category =>
            LazyLoadExtension.LazyLoad(() => Entity?.Category.Map<Category, CategoryViewModel>());
    }
}