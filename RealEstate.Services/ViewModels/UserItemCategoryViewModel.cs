using JetBrains.Annotations;
using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;

namespace RealEstate.Services.ViewModels
{
    public class UserItemCategoryViewModel : BaseLogViewModel<UserItemCategory>
    {
        [JsonIgnore]
        public UserItemCategory Entity { get; private set; }

        [CanBeNull]
        public readonly UserItemCategoryViewModel Instance;

        public UserItemCategoryViewModel(UserItemCategory entity, bool includeDeleted) : base(entity)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Instance = new UserItemCategoryViewModel
            {
                Entity = entity,

                Id = entity.Id,
                Logs = entity.GetLogs()
            };
        }

        public UserItemCategoryViewModel()
        {
        }

        public CategoryViewModel Category { get; set; }
    }
}