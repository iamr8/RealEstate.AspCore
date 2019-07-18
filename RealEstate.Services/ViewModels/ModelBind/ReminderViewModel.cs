using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using System;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class ReminderViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public Reminder Entity { get; }

        public ReminderViewModel(Reminder entity, Action<ReminderViewModel> act = null)
        {
            if (entity == null)
                return;

            Entity = entity;
            act?.Invoke(this);
        }

        public bool IsCheck => Entity?.IsCheck ?? false;
        public string Description => Entity?.Description;
        public DateTime Date => Entity?.Date ?? DateTime.Now;
        public string CheckBank => Entity?.CheckBank;
        public string CheckNumber => Entity?.CheckNumber;
        public decimal Price => Entity?.Price ?? 0;

        public DealViewModel Deal { get; set; }

        public UserViewModel User { get; set; }

        public List<PictureViewModel> Pictures { get; set; }

        public override string ToString()
        {
            return Entity?.ToString();
        }
    }
}