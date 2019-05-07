using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;

namespace RealEstate.Services.ViewModels
{
    public class DealRequestViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public DealRequest Entity { get; }

        public DealRequestViewModel(DealRequest entity, bool includeDeleted, Action<DealRequestViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Entity = entity;
            action?.Invoke(this);
        }

        public void GetItem(bool includeDeleted, Action<ItemViewModel> action = null)
        {
            Item = Entity?.Item.Into(includeDeleted, action).ShowBasedOn(x => x.Property);
        }

        public void GetDeal(bool includeDeleted, Action<DealViewModel> action = null)
        {
            Deal = Entity?.Deal.Into(includeDeleted, action);
        }

        public void GetSms(bool includeDeleted, Action<SmsViewModel> action = null)
        {
            Sms = Entity?.Sms.Into(includeDeleted, action);
        }

        public ItemViewModel Item { get; private set; }
        public DealViewModel Deal { get; private set; }
        public SmsViewModel Sms { get; private set; }
    }
}