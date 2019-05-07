using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;

namespace RealEstate.Services.ViewModels
{
    public class ManagementPercentViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public ManagementPercent Entity { get; }

        public ManagementPercentViewModel(ManagementPercent entity, bool includeDeleted, Action<ManagementPercentViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Entity = entity;
            action?.Invoke(this);
        }

        public int Percent => Entity.Percent;

        public void GetEmployee(bool includeDeleted = false, Action<EmployeeViewModel> action = null)
        {
            Employee = Entity?.Employee.Into(includeDeleted, action);
        }

        public EmployeeViewModel Employee { get; private set; }
    }
}