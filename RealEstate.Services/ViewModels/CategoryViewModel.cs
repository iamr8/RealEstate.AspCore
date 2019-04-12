using JetBrains.Annotations;
using Newtonsoft.Json;
using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using System.Collections.Generic;
using RealEstate.Services.Extensions;

namespace RealEstate.Services.ViewModels
{
    public class CategoryViewModel : BaseLogViewModel<Category>
    {
        [JsonIgnore]
        public Category Entity { get; private set; }

        [CanBeNull]
        public readonly CategoryViewModel Instance;

        public CategoryViewModel(Category entity, bool includeDeleted) : base(entity)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Instance = new CategoryViewModel
            {
                Entity = entity,
                Name = entity.Name,
                Type = entity.Type,
                Id = entity.Id,
                Logs = entity.GetLogs()
            };
        }

        public CategoryViewModel()
        {
        }

        [JsonProperty("nm")]
        public string Name { get; set; }

        [JsonProperty("typ")]
        public CategoryTypeEnum Type { get; set; }

        public List<ItemViewModel> Items { get; set; }
        public List<PropertyViewModel> Properties { get; set; }
    }
}