using JetBrains.Annotations;
using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;

namespace RealEstate.Services.ViewModels
{
    public class UserPropertyCategoryViewModel : BaseLogViewModel<UserPropertyCategory>
    {
        [JsonIgnore]
        public UserPropertyCategory Entity { get; private set; }

        [CanBeNull]
        public readonly UserPropertyCategoryViewModel Instance;

        public UserPropertyCategoryViewModel(UserPropertyCategory entity, bool includeDeleted) : base(entity)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Instance = new UserPropertyCategoryViewModel
            {
                Entity = entity,
                Id = entity.Id,
                Logs = entity.GetLogs()
            };
        }

        public UserPropertyCategoryViewModel()
        {
        }

        public CategoryViewModel Category { get; set; }
    }
}