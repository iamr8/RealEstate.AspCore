using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;

namespace RealEstate.Services.ViewModels
{
    public class UserItemCategoryViewModel : BaseLogViewModel<UserItemCategory>
    {
        [JsonIgnore]
        private readonly UserItemCategory _entity;

        public UserItemCategoryViewModel(UserItemCategory entity, bool includeDeleted, Action<UserItemCategoryViewModel> action = null) : base(entity)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            _entity = entity;
            Id = entity.Id;
            Logs = entity.GetLogs();
            action?.Invoke(this);
        }

        public void GetUser(bool includeDeleted = false, Action<UserViewModel> action = null)
        {
            User = _entity?.User.Into(includeDeleted, action);
        }

        public void GetCategory(bool includeDeleted = false, Action<CategoryViewModel> action = null)
        {
            Category = _entity?.Category.Into(includeDeleted, action);
        }

        public UserViewModel User { get; private set; }
        public CategoryViewModel Category { get; private set; }
    }
}