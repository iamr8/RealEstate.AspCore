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
        private readonly Insurance _entity;

        public InsuranceViewModel(Insurance entity, bool includeDeleted, Action<InsuranceViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            _entity = entity;
            Id = entity.Id;
            Logs = entity.GetLogs();
            action?.Invoke(this);
        }

        public double Price => _entity.Price;

        public void GetEmployee(bool includeDeleted = false, Action<EmployeeViewModel> action = null)
        {
            Employee = _entity?.Employee.Into(includeDeleted, action);
        }

        public EmployeeViewModel Employee { get; private set; }
    }
}