using Newtonsoft.Json;
using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class SmsViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public Sms Entity { get; }

        public SmsViewModel(Sms entity)
        {
            if (entity == null)
                return;

            Entity = entity;
        }

        public string Sender => Entity?.Sender;
        public string Receiver => Entity?.Receiver;
        public string ReferenceId => Entity?.ReferenceId;
        public string Text => Entity?.Text;
        public SmsProvider Provider => Entity?.Provider ?? SmsProvider.KavehNegar;
        public string StatusJson => Entity?.StatusJson;

        public DealRequestViewModel CurrentDealRequest() => DealRequests?.LazyLoadLast();

        public PaymentViewModel CurrentPayment() => Payments?.LazyLoadLast();

        private Lazy<List<DealRequestViewModel>> DealRequests =>
            LazyLoadExtension.LazyLoad(() => Entity?.DealRequests.Map<DealRequest, DealRequestViewModel>());

        private Lazy<List<PaymentViewModel>> Payments =>
            LazyLoadExtension.LazyLoad(() => Entity?.Payments.Map<Payment, PaymentViewModel>());
    }
}