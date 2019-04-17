using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;

namespace RealEstate.Services.ViewModels
{
    public class ItemFeatureViewModel : BaseLogViewModel<ItemFeature>
    {
        private string _value;

        [JsonIgnore]
        private readonly ItemFeature _entity;

        public ItemFeatureViewModel(ItemFeature entity, bool includeDeleted, Action<ItemFeatureViewModel> action = null) : base(entity)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            _entity = entity;
            Id = entity.Id;
            Logs = entity.GetLogs();
            action?.Invoke(this);
        }

        public string Value => _entity.Value.FixCurrency();

        public void GetItem(bool includeDeleted = false, Action<ItemViewModel> action = null)
        {
            Item = _entity?.Item.Into(includeDeleted, action);
        }

        public void GetFeature(bool includeDeleted = false, Action<FeatureViewModel> action = null)
        {
            Feature = _entity?.Feature.Into(includeDeleted, action);
        }

        public ItemViewModel Item { get; private set; }
        public FeatureViewModel Feature { get; private set; }
    }
}