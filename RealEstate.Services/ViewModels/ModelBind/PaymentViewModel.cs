using Newtonsoft.Json;
using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using System;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class PaymentViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public Payment Entity { get; }

        public PaymentViewModel(Payment entity, Action<PaymentViewModel> act = null)
        {
            if (entity == null)
                return;

            Entity = entity;
            act?.Invoke(this);
        }

        public double Value => Entity?.Value ?? 0;

        public string Text => Entity?.Text;

        public PaymentTypeEnum Type => Entity?.Type ?? PaymentTypeEnum.FixedSalary;
        public bool IsCheckedOut => !string.IsNullOrEmpty(CheckoutId);
        public string CheckoutId => Entity?.CheckoutId;

        public EmployeeViewModel Employee { get; set; }

        public PaymentViewModel Checkout { get; set; }

        public SmsViewModel Sms { get; set; }

        public List<PaymentViewModel> Payments { get; set; }

        public List<PictureViewModel> Pictures { get; set; }

        public override string ToString()
        {
            return Entity.ToString();
        }
    }
}