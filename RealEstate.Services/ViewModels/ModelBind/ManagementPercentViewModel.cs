using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using System;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class ManagementPercentViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public ManagementPercent Entity { get; }

        public ManagementPercentViewModel(ManagementPercent entity, Action<ManagementPercentViewModel> act = null)
        {
            if (entity == null)
                return;

            Entity = entity;
            act?.Invoke(this);
        }

        public int Percent => Entity?.Percent ?? 0;

        public EmployeeViewModel Employee { get; set; }

        public override string ToString()
        {
            return Entity.ToString();
        }
    }
}