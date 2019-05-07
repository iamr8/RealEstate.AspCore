using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;

namespace RealEstate.Services.ViewModels
{
    public class PictureViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public Picture Entity { get; }

        public PictureViewModel(Picture entity, bool includeDeleted, Action<PictureViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Entity = entity;
            action?.Invoke(this);
        }

        public string File => Entity.File;
        public string Text => Entity.Text;

        public void GetDeal(bool includeDeleted = false, Action<DealViewModel> action = null)
        {
            Deal = Entity?.Deal.Into(includeDeleted, action);
        }

        public void GetPayment(bool includeDeleted = false, Action<PaymentViewModel> action = null)
        {
            Payment = Entity?.Payment.Into(includeDeleted, action);
        }

        public void GetProperty(bool includeDeleted = false, Action<PropertyViewModel> action = null)
        {
            Property = Entity?.Property.Into(includeDeleted, action);
        }

        public void GetEmployee(bool includeDeleted = false, Action<EmployeeViewModel> action = null)
        {
            Employee = Entity?.Employee.Into(includeDeleted, action);
        }

        public void GetReminder(bool includeDeleted = false, Action<ReminderViewModel> action = null)
        {
            Reminder = Entity?.Reminder.Into(includeDeleted, action);
        }

        public DealViewModel Deal { get; private set; }
        public PaymentViewModel Payment { get; private set; }
        public PropertyViewModel Property { get; private set; }
        public ReminderViewModel Reminder { get; private set; }
        public EmployeeViewModel Employee { get; private set; }
    }
}