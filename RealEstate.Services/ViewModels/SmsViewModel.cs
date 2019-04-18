using Newtonsoft.Json;
using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;

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

        public void GetCustomer(bool includeDeleted = false, Action<CustomerViewModel> action = null)
        {
            Customer = _entity?.Customer.Into(includeDeleted, action);
        }

        public void GetEmployee(bool includeDeleted = false, Action<EmployeeViewModel> action = null)
        {
            Employee = _entity?.Employee.Into(includeDeleted, action);
        }

        public EmployeeViewModel Employee { get; private set; }
        public CustomerViewModel Customer { get; private set; }
    }
}