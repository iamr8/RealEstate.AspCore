using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;
using RealEstate.Base;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class ItemFeatureViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public ItemFeature Entity { get; }

        public ItemFeatureViewModel(ItemFeature entity, Action<ItemFeatureViewModel> act = null)
        {
            if (entity == null)
                return;

            Entity = entity;
            act?.Invoke(this);
        }

        public string Value => Entity?.Value?.FixCurrency();

        public ItemViewModel Item { get; set; }

        public FeatureViewModel Feature { get; set; }

        public override string ToString()
        {
            return Entity.ToString();
        }
    }
}