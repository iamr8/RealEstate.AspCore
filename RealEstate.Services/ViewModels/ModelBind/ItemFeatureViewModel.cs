using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class ItemFeatureViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public ItemFeature Entity { get; }

        public ItemFeatureViewModel(ItemFeature entity)
        {
            if (entity == null)
                return;

            Entity = entity;
        }

        public string Value => Entity?.Value?.FixCurrency();

        public Lazy<ItemViewModel> Item =>
            LazyLoadExtension.LazyLoad(() => Entity?.Item.Into<Item, ItemViewModel>());

        public Lazy<FeatureViewModel> Feature =>
            LazyLoadExtension.LazyLoad(() => Entity?.Feature.Into<Feature, FeatureViewModel>());
    }
}