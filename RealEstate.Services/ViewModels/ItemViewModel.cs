using JetBrains.Annotations;
using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System.Collections.Generic;
using System.Linq;
using RealEstate.Base.Enums;

namespace RealEstate.Services.ViewModels
{
    public class ItemViewModel : BaseLogViewModel<Item>
    {
        [JsonIgnore]
        public Item Entity { get; private set; }

        [CanBeNull]
        public readonly ItemViewModel Instance;

        public ItemViewModel(Item entity, bool includeDeleted) : base(entity)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Instance = new ItemViewModel
            {
                Entity = entity,
                Description = entity.Description,
                IsRequested = entity.Deals?.OrderByDescending(x => x.DateTime).FirstOrDefault()?.Status == DealStatusEnum.Requested,
                Id = entity.Id,
                Logs = entity.GetLogs()
            };
        }

        public ItemViewModel()
        {
        }

        public string Description { get; set; }

        public bool IsRequested { get; set; }
        public CategoryViewModel Category { get; set; }
        public PropertyViewModel Property { get; set; }
        public List<ItemFeatureViewModel> Features { get; set; }
    }
}