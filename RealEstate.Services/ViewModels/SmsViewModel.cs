using Newtonsoft.Json;
using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;

namespace RealEstate.Services.ViewModels
{
    public class SmsViewModel : BaseLogViewModel<Sms>
    {
        [JsonIgnore]
        private readonly Sms _entity;

        public SmsViewModel(Sms entity, bool includeDeleted, Action<SmsViewModel> action = null) : base(entity)
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

        public void GetContact(bool includeDeleted = false, Action<ContactViewModel> action = null)
        {
            Contact = _entity?.Contact.Into(includeDeleted, action);
        }

        public void GetUser(bool includeDeleted = false, Action<UserViewModel> action = null)
        {
            User = _entity?.User.Into(includeDeleted, action);
        }

        public UserViewModel User { get; private set; }
        public ContactViewModel Contact { get; private set; }
    }
}