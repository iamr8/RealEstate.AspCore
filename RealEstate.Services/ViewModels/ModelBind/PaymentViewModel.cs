using Newtonsoft.Json;
using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class PaymentViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public Payment Entity { get; }

        public PaymentViewModel(Payment entity)
        {
            if (entity == null)
                return;

            Entity = entity;
        }

        public double Value => Entity?.Value ?? 0;

        public string Text => Entity?.Text;

        public PaymentTypeEnum Type => Entity?.Type ?? PaymentTypeEnum.FixedSalary;
        public bool IsCheckedOut => !string.IsNullOrEmpty(CheckoutId);
        public string CheckoutId => Entity?.CheckoutId;

        public Lazy<EmployeeViewModel> Employee =>
            LazyLoadExtension.LazyLoad(() => Entity?.Employee.Map<Employee, EmployeeViewModel>());

        public Lazy<PaymentViewModel> Checkout =>
            LazyLoadExtension.LazyLoad(() => Entity?.Checkout.Map<Payment, PaymentViewModel>());

        public Lazy<SmsViewModel> Sms =>
            LazyLoadExtension.LazyLoad(() => Entity?.Sms.Map<Sms, SmsViewModel>());

        public Lazy<List<PaymentViewModel>> Payments =>
            LazyLoadExtension.LazyLoad(() => Entity?.Payments.Map<Payment, PaymentViewModel>());

        public Lazy<List<PictureViewModel>> Pictures =>
            LazyLoadExtension.LazyLoad(() => Entity?.Pictures.Map<Picture, PictureViewModel>());
    }
}