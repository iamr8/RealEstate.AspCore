using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;

namespace RealEstate.Services.ViewModels
{
    public class FixedSalaryViewModel : BaseLogViewModel<FixedSalary>
    {
        [JsonIgnore]
        private readonly FixedSalary _entity;

        public FixedSalaryViewModel(FixedSalary entity, bool includeDeleted, Action<FixedSalaryViewModel> action = null) : base(entity)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            _entity = entity;
            Id = entity.Id;
            Logs = entity.GetLogs();
            action?.Invoke(this);
        }

        public double Value => _entity.Value;

        public void GetUser(bool includeDeleted, Action<UserViewModel> action = null)
        {
            User = _entity?.User.Into(includeDeleted, action);
        }

        public UserViewModel User { get; private set; }
    }
}