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
        private readonly FixedSalary _entity;

        public FixedSalaryViewModel(FixedSalary entity, bool includeDeleted, Action<FixedSalaryViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            _entity = entity;
            Id = entity.Id;
            Logs = entity.GetLogs();
            action?.Invoke(this);
        }

        public double Value => _entity.Value;

        public void GetEmployee(bool includeDeleted, Action<EmployeeViewModel> action = null)
        {
            Employee = _entity?.Employee.Into(includeDeleted, action);
        }

        public EmployeeViewModel Employee { get; private set; }
    }
}