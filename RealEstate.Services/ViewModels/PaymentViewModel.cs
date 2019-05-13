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
        public Payment Entity { get; }

        public PaymentViewModel(Payment entity, bool includeDeleted, Action<PaymentViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Entity = entity;
            action?.Invoke(this);
        }

        public double Value => Entity.Value;

        public string Text => Entity.Text;

        public PaymentTypeEnum Type => Entity.Type;
        public bool IsCheckedOut => !string.IsNullOrEmpty(Entity.CheckoutId);

        public void GetEmployee(bool includeDeleted = false, Action<EmployeeViewModel> action = null)
        {
            Employee = Entity?.Employee.Into(includeDeleted, action);
        }

        public void GetPicture(bool includeDeleted, Action<PictureViewModel> action = null)
        {
            Pictures = Entity?.Pictures.Into(includeDeleted, action);
        }

        public void GetCheckout(bool includeDeleted = false, Action<PaymentViewModel> action = null)
        {
            Checkout = Entity?.Checkout.Into(includeDeleted, action);
        }

        public void GetPayments(bool includeDeleted = false, Action<PaymentViewModel> action = null)
        {
            Payments = Entity?.Payments.Into(includeDeleted, action);
        }

        public void GetSms(bool includeDeleted = false, Action<SmsViewModel> action = null)
        {
            Sms = Entity?.Sms.Into(includeDeleted, action);
        }

        public EmployeeViewModel Employee { get; private set; }

        public List<PictureViewModel> Pictures { get; private set; }
        public PaymentViewModel Checkout { get; private set; }
        public List<PaymentViewModel> Payments { get; private set; }
        public SmsViewModel Sms { get; private set; }
    }
}