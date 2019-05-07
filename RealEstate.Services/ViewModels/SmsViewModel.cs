using Newtonsoft.Json;
using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RealEstate.Services.ViewModels
{
    public class SmsViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public Sms Entity { get; }

        public SmsViewModel(Sms entity, bool includeDeleted, Action<SmsViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Entity = entity;

            action?.Invoke(this);
        }

        public string Sender => Entity.Sender;
        public string Receiver => Entity.Receiver;
        public string ReferenceId => Entity.ReferenceId;
        public string Text => Entity.Text;
        public SmsProvider Provider => Entity.Provider;
        public string StatusJson => Entity.StatusJson;

        public void GetDealRequests(bool includeDeleted = false, Action<DealRequestViewModel> action = null)
        {
            DealRequests = Entity?.DealRequests.Into(includeDeleted, action);
        }

        public void GetPayments(bool includeDeleted = false, Action<PaymentViewModel> action = null)
        {
            Payments = Entity?.Payments.Into(includeDeleted, action);
        }

        private List<DealRequestViewModel> DealRequests { get; set; }
        public DealRequestViewModel CurrentDealRequest => DealRequests?.OrderDescendingByCreationDateTime().FirstOrDefault();
        private List<PaymentViewModel> Payments { get; set; }
        public PaymentViewModel CurrentPayment => Payments?.OrderDescendingByCreationDateTime().FirstOrDefault();
    }
}