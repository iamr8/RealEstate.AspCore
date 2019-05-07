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
        public Presence Entity { get; }

        public PresenceViewModel(Presence entity, bool includeDeleted, Action<PresenceViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Entity = entity;
            action?.Invoke(this);
        }

        public PresenseStatusEnum Status => Entity.Status;
        public DateTime Date => Entity.Date;
        public int Hour => Entity.Hour;
        public int Minute => Entity.Minute;

        public void GetEmployee(bool includeDeleted = false, Action<EmployeeViewModel> action = null)
        {
            Employee = Entity?.Employee.Into(includeDeleted, action);
        }

        public EmployeeViewModel Employee { get; private set; }
    }
}