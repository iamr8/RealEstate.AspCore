using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels
{
    public class DivisionViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        private readonly Division _entity;

        public DivisionViewModel(Division entity, bool includeDeleted, Action<DivisionViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            _entity = entity;
            Id = entity.Id;
            Logs = entity.GetLogs();
            action?.Invoke(this);
        }

        public void GetEmployeeDivisions(bool includeDeleted = false, Action<EmployeeDivisionViewModel> action = null)
        {
            EmployeeDivisions = _entity?.EmployeeDivisions.Into(includeDeleted, action);
        }

        public string Subject => _entity.Subject;
        public List<EmployeeDivisionViewModel> EmployeeDivisions { get; private set; }
    }
}