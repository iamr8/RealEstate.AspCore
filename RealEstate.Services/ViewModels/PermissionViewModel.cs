using JetBrains.Annotations;
using Newtonsoft.Json;
using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;

namespace RealEstate.Services.ViewModels
{
    public class PermissionViewModel : BaseLogViewModel<Permission>
    {
        [JsonIgnore]
        public Permission Entity { get; private set; }

        [CanBeNull]
        public readonly PermissionViewModel Instance;

        public PermissionViewModel()
        {
        }

        public PermissionViewModel(Permission entity, bool includeDeleted) : base(entity)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Instance = new PermissionViewModel
            {
                Entity = entity,
                Key = entity.Key,
                Id = entity.Id,
                Type = entity.Type,
                Logs = entity.GetLogs()
            };
        }

        public string Key { get; set; }
        public PermissionTypeEnum Type { get; set; }
    }
}