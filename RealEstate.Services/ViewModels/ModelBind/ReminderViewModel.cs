using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class ReminderViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public Reminder Entity { get; }

        public ReminderViewModel(Reminder entity)
        {
            if (entity == null)
                return;

            Entity = entity;
        }

        public bool IsCheck => Entity?.IsCheck ?? false;
        public string Description => Entity?.Description;
        public DateTime Date => Entity?.Date ?? DateTime.Now;
        public string CheckBank => Entity?.CheckBank;
        public string CheckNumber => Entity?.CheckNumber;
        public double Price => (double)(Entity?.Price ?? 0);

        public Lazy<DealViewModel> Deal =>
            LazyLoadExtension.LazyLoad(() => Entity?.Deal.Map<Deal, DealViewModel>());

        public Lazy<UserViewModel> User =>
            LazyLoadExtension.LazyLoad(() => Entity?.User.Map<User, UserViewModel>());

        public Lazy<List<PictureViewModel>> Pictures =>
            LazyLoadExtension.LazyLoad(() => Entity?.Pictures.Map<Picture, PictureViewModel>());
    }
}