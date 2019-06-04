using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using System;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class UserPropertyCategoryViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public UserPropertyCategory Entity { get; }

        public UserPropertyCategoryViewModel(UserPropertyCategory entity, Action<UserPropertyCategoryViewModel> act = null)
        {
            if (entity == null)
                return;

            Entity = entity;
            act?.Invoke(this);
        }

        public UserViewModel User { get; set; }

        public CategoryViewModel Category { get; set; }

        public override string ToString()
        {
            return Entity?.ToString();
        }
    }
}