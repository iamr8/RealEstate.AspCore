using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using System;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class LeaveViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public Leave Entity { get; }

        public LeaveViewModel(Leave entity, Action<LeaveViewModel> act = null)
        {
            if (entity == null)
                return;

            Entity = entity;
            act?.Invoke(this);
        }

        public DateTime From => Entity?.From ?? DateTime.Now;
        public DateTime To => Entity?.To ?? DateTime.Now;
        public string Reason => Entity?.Reason;

        public EmployeeViewModel Employee { get; set; }

        public override string ToString()
        {
            return Entity.ToString();
        }
    }
}