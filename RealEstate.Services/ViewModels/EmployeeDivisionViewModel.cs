using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;

namespace RealEstate.Services.ViewModels
{
    public class EmployeeDivisionViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        private readonly EmployeeDivision _entity;

        public EmployeeDivisionViewModel(EmployeeDivision entity, bool includeDeleted, Action<EmployeeDivisionViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            _entity = entity;
            Id = entity.Id;
            Logs = entity.GetLogs();
            action?.Invoke(this);
        }

        public void GetEmployee(bool includeDeleted = false, Action<EmployeeViewModel> action = null)
        {
            Employee = _entity?.Employee.Into(includeDeleted, action);
        }

        public void GetDivision(bool includeDeleted = false, Action<DivisionViewModel> action = null)
        {
            Division = _entity?.Division.Into(includeDeleted, action);
        }

        public EmployeeViewModel Employee { get; private set; }
        public DivisionViewModel Division { get; private set; }
    }
}