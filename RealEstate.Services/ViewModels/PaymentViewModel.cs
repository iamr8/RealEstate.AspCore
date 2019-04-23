using Newtonsoft.Json;
using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels
{
    public class PaymentViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        private readonly Payment _entity;

        public PaymentViewModel(Payment entity, bool includeDeleted, Action<PaymentViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            _entity = entity;
            Id = entity.Id;
            Logs = entity.GetLogs();
            action?.Invoke(this);
        }

        public double Value => _entity.Value;

        public string Text => _entity.Text;

        public PaymentTypeEnum Type => _entity.Type;

        public void GetEmployee(bool includeDeleted = false, Action<EmployeeViewModel> action = null)
        {
            Employee = _entity?.Employee.Into(includeDeleted, action);
        }

        public void GetPicture(bool includeDeleted, Action<PictureViewModel> action = null)
        {
            Pictures = _entity?.Pictures.Into(includeDeleted, action);
        }

        public void GetCheckout(bool includeDeleted = false, Action<PaymentViewModel> action = null)
        {
            Checkout = _entity?.Checkout.Into(includeDeleted, action);
        }

        public void GetPayments(bool includeDeleted = false, Action<PaymentViewModel> action = null)
        {
            Payments = _entity?.Payments.Into(includeDeleted, action);
        }

        public void GetSms(bool includeDeleted = false, Action<SmsViewModel> action = null)
        {
            Sms = _entity?.Sms.Into(includeDeleted, action);
        }

        public EmployeeViewModel Employee { get; private set; }

        public List<PictureViewModel> Pictures { get; private set; }
        public PaymentViewModel Checkout { get; private set; }
        public List<PaymentViewModel> Payments { get; private set; }
        public SmsViewModel Sms { get; private set; }
    }
}