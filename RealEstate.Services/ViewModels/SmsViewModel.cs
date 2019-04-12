using JetBrains.Annotations;
using Newtonsoft.Json;
using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;

namespace RealEstate.Services.ViewModels
{
    public class SmsViewModel : BaseLogViewModel<Sms>
    {
        [JsonIgnore]
        public Sms Entity { get; private set; }

        [CanBeNull]
        public readonly SmsViewModel Instance;

        public SmsViewModel()
        {
        }

        public SmsViewModel(Sms entity, bool includeDeleted) : base(entity)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Instance = new SmsViewModel
            {
                Entity = entity,
                Id = entity.Id,
                Receiver = entity.Receiver,
                Provider = entity.Provider,
                ReferenceId = entity.ReferenceId,
                Sender = entity.Sender,
                StatusJson = entity.StatusJson,
                Text = entity.Text,
                Logs = entity.GetLogs()
            };
        }

        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string ReferenceId { get; set; }
        public string Text { get; set; }
        public SmsProvider Provider { get; set; }
        public string StatusJson { get; set; }
        public UserViewModel User { get; set; }
        public ContactViewModel Contact { get; set; }
        public SmsTemplateViewModel SmsTemplate { get; set; }
    }
}