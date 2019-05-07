using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;

namespace RealEstate.Services.ViewModels
{
    public class LeaveViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public Leave Entity { get; }

        public LeaveViewModel(Leave entity, bool includeDeleted, Action<LeaveViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Entity = entity;
            action?.Invoke(this);
        }

        public DateTime From => Entity.From;
        public DateTime To => Entity.To;
        public string Reason => Entity.Reason;

        public void GetEmployee(bool includeDeleted = false, Action<EmployeeViewModel> action = null)
        {
            Employee = Entity?.Employee.Into(includeDeleted, action);
        }

        public EmployeeViewModel Employee { get; private set; }
    }
}