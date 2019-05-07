using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;

namespace RealEstate.Services.ViewModels
{
    public class FixedSalaryViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public FixedSalary Entity { get; }

        public FixedSalaryViewModel(FixedSalary entity, bool includeDeleted, Action<FixedSalaryViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Entity = entity;
            action?.Invoke(this);
        }

        public double Value => Entity.Value;

        public void GetEmployee(bool includeDeleted, Action<EmployeeViewModel> action = null)
        {
            Employee = Entity?.Employee.Into(includeDeleted, action);
        }

        public EmployeeViewModel Employee { get; private set; }
    }
}