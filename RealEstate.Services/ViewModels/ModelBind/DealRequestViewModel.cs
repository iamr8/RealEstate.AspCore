using Newtonsoft.Json;
using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using System;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class DealRequestViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public DealRequest Entity { get; }

        public DealRequestViewModel(DealRequest entity, Action<DealRequestViewModel> act = null)
        {
            if (entity == null)
                return;

            Entity = entity;
            act?.Invoke(this);
        }

        public DealStatusEnum Status => Entity?.Status ?? DealStatusEnum.Rejected;

        public ItemViewModel Item { get; set; }
        public DealViewModel Deal { get; set; }
        public SmsViewModel Sms { get; set; }
    }
}