using Newtonsoft.Json;
using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class SmsViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public Sms Entity { get; }

        public SmsViewModel(Sms entity, Action<SmsViewModel> act = null)
        {
            if (entity == null)
                return;

            Entity = entity;
            act?.Invoke(this);
        }

        public string Sender => Entity?.Sender;
        public string Receiver => Entity?.Receiver;
        public string ReferenceId => Entity?.ReferenceId;
        public string Text => Entity?.Text;
        public SmsProvider Provider => Entity?.Provider ?? SmsProvider.KavehNegar;
        public string StatusJson => Entity?.StatusJson;

        public DealRequestViewModel CurrentDealRequest() => DealRequests?.OrderDescendingByCreationDateTime().FirstOrDefault();

        public PaymentViewModel CurrentPayment => Payments?.OrderDescendingByCreationDateTime().FirstOrDefault();

        private List<DealRequestViewModel> DealRequests { get; set; }

        private List<PaymentViewModel> Payments { get; set; }

        public override string ToString()
        {
            return Entity?.ToString();
        }
    }
}