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
        private readonly DealRequest _entity;

        public DealRequestViewModel(DealRequest entity, bool includeDeleted, Action<DealRequestViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            _entity = entity;
            Id = entity.Id;
            Logs = entity.GetLogs();
            action?.Invoke(this);
        }

        public void GetItem(bool includeDeleted, Action<ItemViewModel> action = null)
        {
            Item = _entity?.Item.Into(includeDeleted, action).ShowBasedOn(x => x.Property);
        }

        public void GetDeal(bool includeDeleted, Action<DealViewModel> action = null)
        {
            Deal = _entity?.Deal.Into(includeDeleted, action);
        }

        public ItemViewModel Item { get; private set; }
        public DealViewModel Deal { get; private set; }
    }
}