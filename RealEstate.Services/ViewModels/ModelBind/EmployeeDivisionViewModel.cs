using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using System;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class EmployeeDivisionViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public EmployeeDivision Entity { get; }

        public EmployeeDivisionViewModel(EmployeeDivision entity, Action<EmployeeDivisionViewModel> act = null)
        {
            if (entity == null)
                return;

            Entity = entity;
            act?.Invoke(this);
        }

        public EmployeeViewModel Employee { get; set; }
        public DivisionViewModel Division { get; set; }

        public override string ToString()
        {
            return Entity.ToString();
        }
    }
}