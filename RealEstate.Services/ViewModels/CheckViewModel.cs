using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels
{
    public class CheckViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        private readonly Check _entity;

        public CheckViewModel(Check entity, bool includeDeleted, Action<CheckViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            _entity = entity;
            Id = entity.Id;
            Logs = entity.GetLogs();
            action?.Invoke(this);
        }

        public DateTime PayDate => _entity.PayDate;
        public string Bank => _entity.Bank;
        public string CheckNumber => _entity.CheckNumber;
        public string Price => _entity.Price;
        public string Description => _entity.Description;

        public void GetDeal(bool includeDeleted = false, Action<DealViewModel> action = null)
        {
            Deal = _entity?.Deal.Into(includeDeleted, action);
        }

        public void GetPictures(bool includeDeleted = false, Action<PictureViewModel> action = null)
        {
            Pictures = _entity?.Pictures.Into(includeDeleted, action);
        }

        public void GetReminder(bool includeDeleted = false, Action<ReminderViewModel> action = null)
        {
            Reminder = _entity?.Reminder.Into(includeDeleted, action);
        }

        public DealViewModel Deal { get; private set; }
        public ReminderViewModel Reminder { get; private set; }
        public List<PictureViewModel> Pictures { get; private set; }
    }
}