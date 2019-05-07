using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;

namespace RealEstate.Services.ViewModels
{
    public class ItemFeatureViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public ItemFeature Entity { get; }

        public ItemFeatureViewModel(ItemFeature entity, bool includeDeleted, Action<ItemFeatureViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Entity = entity;
            action?.Invoke(this);
        }

        public string Value => Entity?.Value?.FixCurrency();

        public void GetItem(bool includeDeleted = false, Action<ItemViewModel> action = null)
        {
            Item = Entity?.Item.Into(includeDeleted, action);
        }

        public void GetFeature(bool includeDeleted = false, Action<FeatureViewModel> action = null)
        {
            Feature = Entity?.Feature.Into(includeDeleted, action);
        }

        public ItemViewModel Item { get; private set; }
        public FeatureViewModel Feature { get; private set; }
    }
}