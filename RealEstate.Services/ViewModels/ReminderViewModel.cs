using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;

namespace RealEstate.Services.ViewModels
{
    public class ReminderViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        private readonly Reminder _entity;

        public ReminderViewModel(Reminder entity, bool includeDeleted, Action<ReminderViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            _entity = entity;
            Id = entity.Id;
            Logs = entity.GetLogs();
            action?.Invoke(this);
        }

        public string Text => _entity.Text;
        public DateTime AlarmTime => _entity.AlarmTime;

        public void GetUser(bool includeDeleted = false, Action<UserViewModel> action = null)
        {
            User = _entity?.User.Into(includeDeleted, action);
        }

        public UserViewModel User { get; private set; }
    }
}