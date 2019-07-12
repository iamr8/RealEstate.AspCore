using Newtonsoft.Json;
using RealEstate.Base;
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

        public ItemFeatureViewModel(ItemFeature entity, Action<ItemFeatureViewModel> act = null)
        {
            if (entity == null)
                return;

            Entity = entity;
            act?.Invoke(this);
        }

        private FeatureFixExtension.NormalizeFeatureStatus Normalized => Feature != null
            ? Entity?.Value.FixPersian().NormalizeFeature(Feature.Name)
            : new FeatureFixExtension.NormalizeFeatureStatus(Entity?.Value.FixPersian(), Entity?.Value.FixPersian());

        public string Value => Normalized.Value;
        public string OriginalValue => Normalized.OriginalValue;
        public ItemViewModel Item { get; set; }

        public FeatureViewModel Feature { get; set; }

        public override string ToString()
        {
            return Entity.ToString();
        }
    }
}