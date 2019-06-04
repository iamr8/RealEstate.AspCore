using Newtonsoft.Json;
using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using System;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class PresenceViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public Presence Entity { get; }

        public PresenceViewModel(Presence entity, Action<PresenceViewModel> act = null)
        {
            if (entity == null)
                return;

            Entity = entity;
            act?.Invoke(this);
        }

        public PresenseStatusEnum Status => Entity?.Status ?? PresenseStatusEnum.End;
        public DateTime Date => Entity?.Date ?? DateTime.Now;
        public int Hour => Entity?.Hour ?? 0;
        public int Minute => Entity?.Minute ?? 0;

        public EmployeeViewModel Employee { get; set; }

        public override string ToString()
        {
            return Entity.ToString();
        }
    }
}