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
        public Division Entity { get; }

        public DivisionViewModel(Division entity, bool includeDeleted, Action<DivisionViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Entity = entity;
            action?.Invoke(this);
        }

        public void GetEmployeeDivisions(bool includeDeleted = false, Action<EmployeeDivisionViewModel> action = null)
        {
            EmployeeDivisions = Entity?.EmployeeDivisions.Into(includeDeleted, action);
        }

        public string Name => Entity.Name;
        public List<EmployeeDivisionViewModel> EmployeeDivisions { get; private set; }
    }
}