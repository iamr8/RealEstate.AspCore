using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class ManagementPercentViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public ManagementPercent Entity { get; }

        public ManagementPercentViewModel(ManagementPercent entity)
        {
            if (entity == null)
                return;

            Entity = entity;
        }

        public int Percent => Entity?.Percent ?? 0;

        public Lazy<EmployeeViewModel> Employee =>
            LazyLoadExtension.LazyLoad(() => Entity?.Employee.Into<Employee, EmployeeViewModel>());
    }
}