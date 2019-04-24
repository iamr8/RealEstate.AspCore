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

        public bool IsCheck => _entity?.IsCheck ?? false;
        public string Description => _entity?.Description;
        public DateTime Date => _entity?.Date ?? DateTime.Now;
        public string CheckBank => _entity?.CheckBank;
        public string CheckNumber => _entity?.CheckNumber;
        public double Price => (double)(_entity?.Price ?? 0);

        public void GetDeal(bool includeDeleted = false, Action<DealViewModel> action = null)
        {
            Deal = _entity?.Deal.Into(includeDeleted, action);
        }

        public void GetUser(bool includeDeleted = false, Action<UserViewModel> action = null)
        {
            User = _entity?.User.Into(includeDeleted, action);
        }

        public void GetPictures(bool includeDeleted = false, Action<PictureViewModel> action = null)
        {
            Pictures = _entity?.Pictures.Into(includeDeleted, action);
        }

        public DealViewModel Deal { get; private set; }
        public UserViewModel User { get; private set; }
        public List<PictureViewModel> Pictures { get; private set; }
    }
}