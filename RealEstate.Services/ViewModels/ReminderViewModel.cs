using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;
using System.Collections.Generic;

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

        public string Description => _entity.Description;
        public DateTime Date => _entity.Date;

        public void GetUser(bool includeDeleted = false, Action<UserViewModel> action = null)
        {
            User = _entity?.User.Into(includeDeleted, action);
        }

        public void GetChecks(bool includeDeleted = false, Action<CheckViewModel> action = null)
        {
            Checks = _entity?.Checks.Into(includeDeleted, action);
        }

        public UserViewModel User { get; private set; }
        public List<CheckViewModel> Checks { get; private set; }
    }
}