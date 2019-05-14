using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class DivisionViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public Division Entity { get; }

        public DivisionViewModel(Division entity)
        {
            if (entity == null)
                return;

            Entity = entity;
        }

        public string Name => Entity?.Name;
        public Lazy<List<EmployeeDivisionViewModel>> EmployeeDivisions => LazyLoadExtension.LazyLoad(() => Entity?.EmployeeDivisions.Into<EmployeeDivision, EmployeeDivisionViewModel>());
    }
}