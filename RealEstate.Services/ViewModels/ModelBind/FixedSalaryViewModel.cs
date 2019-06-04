using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using System;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class FixedSalaryViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public FixedSalary Entity { get; }

        public FixedSalaryViewModel(FixedSalary entity, Action<FixedSalaryViewModel> act = null)
        {
            if (entity == null)
                return;

            Entity = entity;
            act?.Invoke(this);
        }

        public double Value => Entity?.Value ?? 0;

        public EmployeeViewModel Employee { get; set; }

        public override string ToString()
        {
            return Entity.ToString();
        }
    }
}