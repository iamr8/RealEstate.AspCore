using Newtonsoft.Json;
using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;

namespace RealEstate.Services.ViewModels
{
    public class PresenceViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        private readonly Presence _entity;

        public PresenceViewModel(Presence entity, bool includeDeleted, Action<PresenceViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            _entity = entity;
            Id = entity.Id;
            Logs = entity.GetLogs();
            action?.Invoke(this);
        }

        public PresenseStatusEnum Status => _entity.Status;
        public int Year => _entity.Year;
        public int Month => _entity.Month;
        public int Day => _entity.Day;
        public int Hour => _entity.Hour;
        public int Minute => _entity.Minute;

        public void GetEmployee(bool includeDeleted = false, Action<EmployeeViewModel> action = null)
        {
            Employee = _entity?.Employee.Into(includeDeleted, action);
        }

        public EmployeeViewModel Employee { get; private set; }
    }
}