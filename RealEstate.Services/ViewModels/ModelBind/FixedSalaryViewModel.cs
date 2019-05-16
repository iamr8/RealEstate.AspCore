using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class FixedSalaryViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public FixedSalary Entity { get; }

        public FixedSalaryViewModel(FixedSalary entity)
        {
            if (entity == null)
                return;

            Entity = entity;
        }

        public double Value => Entity?.Value ?? 0;

        public Lazy<EmployeeViewModel> Employee =>
            LazyLoadExtension.LazyLoad(() => Entity?.Employee.Map<Employee, EmployeeViewModel>());
    }
}