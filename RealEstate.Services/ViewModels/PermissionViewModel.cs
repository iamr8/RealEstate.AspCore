using Newtonsoft.Json;
using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;

namespace RealEstate.Services.ViewModels
{
    public class PermissionViewModel : BaseLogViewModel<Permission>
    {
        [JsonIgnore]
        private readonly Permission _entity;

        public PermissionViewModel(Permission entity, bool includeDeleted, Action<PermissionViewModel> action = null) : base(entity)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            _entity = entity;
            Id = entity.Id;
            Logs = entity.GetLogs();
        }

        public string Key => _entity?.Key;
        public PermissionTypeEnum? Type => _entity?.Type;

        public void GetUser(bool includeDeleted, Action<UserViewModel> action = null)
        {
            User = _entity?.User.Into(includeDeleted, action);
        }

        public UserViewModel User { get; private set; }
    }
}