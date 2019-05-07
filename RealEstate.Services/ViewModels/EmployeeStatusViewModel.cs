using Newtonsoft.Json;
using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;

namespace RealEstate.Services.ViewModels
{
    public class EmployeeStatusViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public EmployeeStatus Entity { get; }

        public EmployeeStatusViewModel(EmployeeStatus entity, bool includeDeleted, Action<EmployeeStatusViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Entity = entity;
            action?.Invoke(this);
        }

        public EmployeeStatusEnum Status => Entity.Status;

        public void GetEmployee(bool includeDeleted = false, Action<EmployeeViewModel> action = null)
        {
            Employee = Entity?.Employee.Into(includeDeleted, action);
        }

        public EmployeeViewModel Employee { get; private set; }
    }
}