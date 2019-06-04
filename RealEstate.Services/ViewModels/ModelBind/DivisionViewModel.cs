using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using System;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class DivisionViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public Division Entity { get; }

        public DivisionViewModel(Division entity, Action<DivisionViewModel> act = null)
        {
            if (entity == null)
                return;

            Entity = entity;
            act?.Invoke(this);
        }

        public string Name => Entity?.Name;
        public List<EmployeeDivisionViewModel> EmployeeDivisions { get; set; }

        public override string ToString()
        {
            return Entity.ToString();
        }
    }
}