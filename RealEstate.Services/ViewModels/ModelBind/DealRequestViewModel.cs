using Newtonsoft.Json;
using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class DealRequestViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public DealRequest Entity { get; }

        public DealRequestViewModel(DealRequest entity)
        {
            if (entity == null)
                return;

            Entity = entity;
        }

        public DealStatusEnum Status => Entity?.Status ?? DealStatusEnum.Rejected;

        public Lazy<ItemViewModel> Item => LazyLoadExtension.LazyLoad(() => Entity?.Item.Into<Item, ItemViewModel>());
        public Lazy<DealViewModel> Deal => LazyLoadExtension.LazyLoad(() => Entity?.Deal.Into<Deal, DealViewModel>());
        public Lazy<SmsViewModel> Sms => LazyLoadExtension.LazyLoad(() => Entity?.Sms.Into<Sms, SmsViewModel>());
    }
}