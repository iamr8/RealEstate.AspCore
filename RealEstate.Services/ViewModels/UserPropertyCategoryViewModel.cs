using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;

namespace RealEstate.Services.ViewModels
{
    public class UserPropertyCategoryViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public UserPropertyCategory Entity { get; }

        public UserPropertyCategoryViewModel(UserPropertyCategory entity, bool includeDeleted, Action<UserPropertyCategoryViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Entity = entity;

            action?.Invoke(this);
        }

        public void GetUser(bool includeDeleted = false, Action<UserViewModel> action = null)
        {
            User = Entity?.User.Into(includeDeleted, action);
        }

        public void GetCategory(bool includeDeleted = false, Action<CategoryViewModel> action = null)
        {
            Category = Entity?.Category.Into(includeDeleted, action);
        }

        public UserViewModel User { get; private set; }
        public CategoryViewModel Category { get; private set; }
    }
}