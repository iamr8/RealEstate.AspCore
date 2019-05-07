using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;

namespace RealEstate.Services.ViewModels
{
    public class InsuranceViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public Insurance Entity { get; }

        public InsuranceViewModel(Insurance entity, bool includeDeleted, Action<InsuranceViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Entity = entity;
            action?.Invoke(this);
        }

        public double Price => Entity.Price;

        public void GetEmployee(bool includeDeleted = false, Action<EmployeeViewModel> action = null)
        {
            Employee = Entity?.Employee.Into(includeDeleted, action);
        }

        public EmployeeViewModel Employee { get; private set; }
    }
}