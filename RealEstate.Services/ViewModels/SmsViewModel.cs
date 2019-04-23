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
        private readonly Sms _entity;

        public SmsViewModel(Sms entity, bool includeDeleted, Action<SmsViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            _entity = entity;
            Id = entity.Id;
            Logs = entity.GetLogs();
            action?.Invoke(this);
        }

        public string Sender => _entity.Sender;
        public string Receiver => _entity.Receiver;
        public string ReferenceId => _entity.ReferenceId;
        public string Text => _entity.Text;
        public SmsProvider Provider => _entity.Provider;
        public string StatusJson => _entity.StatusJson;

        public void GetDealRequests(bool includeDeleted = false, Action<DealRequestViewModel> action = null)
        {
            DealRequests = _entity?.DealRequests.Into(includeDeleted, action);
        }

        public void GetPayments(bool includeDeleted = false, Action<PaymentViewModel> action = null)
        {
            Payments = _entity?.Payments.Into(includeDeleted, action);
        }

        private List<DealRequestViewModel> DealRequests { get; set; }
        public DealRequestViewModel CurrentDealRequest => DealRequests?.OrderDescendingByCreationDateTime().FirstOrDefault();
        private List<PaymentViewModel> Payments { get; set; }
        public PaymentViewModel CurrentPayment => Payments?.OrderDescendingByCreationDateTime().FirstOrDefault();
    }
}