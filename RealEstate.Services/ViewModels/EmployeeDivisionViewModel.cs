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
        public EmployeeDivision Entity { get; }

        public EmployeeDivisionViewModel(EmployeeDivision entity, bool includeDeleted, Action<EmployeeDivisionViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Entity = entity;
            action?.Invoke(this);
        }

        public void GetEmployee(bool includeDeleted = false, Action<EmployeeViewModel> action = null)
        {
            Employee = Entity?.Employee.Into(includeDeleted, action);
        }

        public void GetDivision(bool includeDeleted = false, Action<DivisionViewModel> action = null)
        {
            Division = Entity?.Division.Into(includeDeleted, action);
        }

        public EmployeeViewModel Employee { get; private set; }
        public DivisionViewModel Division { get; private set; }
    }
}