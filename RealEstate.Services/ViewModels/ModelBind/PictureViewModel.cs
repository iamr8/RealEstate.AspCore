using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class PictureViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public Picture Entity { get; }

        public PictureViewModel(Picture entity)
        {
            if (entity == null)
                return;

            Entity = entity;
        }

        public string File => Entity?.File;
        public string Text => Entity?.Text;

        public Lazy<DealViewModel> Deal =>
            LazyLoadExtension.LazyLoad(() => Entity?.Deal.Into<Deal, DealViewModel>());

        public Lazy<PaymentViewModel> Payment =>
            LazyLoadExtension.LazyLoad(() => Entity?.Payment.Into<Payment, PaymentViewModel>());

        public Lazy<PropertyViewModel> Property =>
            LazyLoadExtension.LazyLoad(() => Entity?.Property.Into<Property, PropertyViewModel>());

        public Lazy<ReminderViewModel> Reminder =>
            LazyLoadExtension.LazyLoad(() => Entity?.Reminder.Into<Reminder, ReminderViewModel>());

        public Lazy<EmployeeViewModel> Employee =>
            LazyLoadExtension.LazyLoad(() => Entity?.Employee.Into<Employee, EmployeeViewModel>());
    }
}