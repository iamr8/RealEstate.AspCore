using JetBrains.Annotations;
using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;

namespace RealEstate.Services.ViewModels
{
    public class ReminderViewModel : BaseLogViewModel<Reminder>
    {
        [JsonIgnore]
        public Reminder Entity { get; private set; }

        [CanBeNull]
        public readonly ReminderViewModel Instance;

        public ReminderViewModel()
        {
        }

        public ReminderViewModel(Reminder entity, bool includeDeleted) : base(entity)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Instance = new ReminderViewModel
            {
                Entity = entity,
                Id = entity.Id,
                Logs = entity.GetLogs(),
                AlarmTime = entity.AlarmTime,
                Text = entity.Text
            };
        }

        public string Text { get; set; }
        public DateTime AlarmTime { get; set; }
    }
}