using Newtonsoft.Json;
using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RealEstate.Services.ViewModels
{
    public class ItemViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        private readonly Item _entity;

        public ItemViewModel(Item entity, bool includeDeleted, Action<ItemViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            _entity = entity;
            Id = entity.Id;
            Logs = entity.GetLogs();
            action?.Invoke(this);
        }

        public string Description => _entity.Description;

        public bool IsRequested => _entity.Deals?.LastOrDefault()?.Status == DealStatusEnum.Requested;

        public void GetCategory(bool includeDeleted = false, Action<CategoryViewModel> action = null)
        {
            Category = _entity?.Category.Into(includeDeleted, action);
        }

        public void GetProperty(bool includeDeleted = false, Action<PropertyViewModel> action = null)
        {
            Property = _entity?.Property.Into(includeDeleted, action);
        }

        public void GetItemFeatures(bool includeDeleted = false, Action<ItemFeatureViewModel> action = null)
        {
            ItemFeatures = _entity?.ItemFeatures.Into(includeDeleted, action);
        }

        public void GetDeals(bool includeDeleted = false, Action<DealViewModel> action = null)
        {
            Deals = _entity?.Deals?.Into(includeDeleted, action);
        }

        public CategoryViewModel Category { get; private set; }
        public PropertyViewModel Property { get; private set; }
        public List<ItemFeatureViewModel> ItemFeatures { get; private set; }
        public List<DealViewModel> Deals { get; private set; }
    }
}