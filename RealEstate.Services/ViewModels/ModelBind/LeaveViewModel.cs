using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class LeaveViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public Leave Entity { get; }

        public LeaveViewModel(Leave entity)
        {
            if (entity == null)
                return;

            Entity = entity;
        }

        public DateTime From => Entity?.From ?? DateTime.Now;
        public DateTime To => Entity?.To ?? DateTime.Now;
        public string Reason => Entity?.Reason;

        public Lazy<EmployeeViewModel> Employee =>
            LazyLoadExtension.LazyLoad(() => Entity?.Employee.Into<Employee, EmployeeViewModel>());
    }
}