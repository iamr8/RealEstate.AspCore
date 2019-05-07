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
        public Reminder Entity { get; }

        public ReminderViewModel(Reminder entity, bool includeDeleted, Action<ReminderViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Entity = entity;
            action?.Invoke(this);
        }

        public bool IsCheck => Entity?.IsCheck ?? false;
        public string Description => Entity?.Description;
        public DateTime Date => Entity?.Date ?? DateTime.Now;
        public string CheckBank => Entity?.CheckBank;
        public string CheckNumber => Entity?.CheckNumber;
        public double Price => (double)(Entity?.Price ?? 0);

        public void GetDeal(bool includeDeleted = false, Action<DealViewModel> action = null)
        {
            Deal = Entity?.Deal.Into(includeDeleted, action);
        }

        public void GetUser(bool includeDeleted = false, Action<UserViewModel> action = null)
        {
            User = Entity?.User.Into(includeDeleted, action);
        }

        public void GetPictures(bool includeDeleted = false, Action<PictureViewModel> action = null)
        {
            Pictures = Entity?.Pictures.Into(includeDeleted, action);
        }

        public DealViewModel Deal { get; private set; }
        public UserViewModel User { get; private set; }
        public List<PictureViewModel> Pictures { get; private set; }
    }
}