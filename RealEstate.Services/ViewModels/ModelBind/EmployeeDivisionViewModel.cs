using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class EmployeeDivisionViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public EmployeeDivision Entity { get; }

        public EmployeeDivisionViewModel(EmployeeDivision entity)
        {
            if (entity == null)
                return;

            Entity = entity;
        }

        public Lazy<EmployeeViewModel> Employee => LazyLoadExtension.LazyLoad(() => Entity?.Employee.Into<Employee, EmployeeViewModel>());
        public Lazy<DivisionViewModel> Division => LazyLoadExtension.LazyLoad(() => Entity?.Division.Into<Division, DivisionViewModel>());
    }
}