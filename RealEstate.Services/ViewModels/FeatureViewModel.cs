using JetBrains.Annotations;
using Newtonsoft.Json;
using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;

namespace RealEstate.Services.ViewModels
{
    public class FeatureViewModel : BaseLogViewModel<Feature>
    {
        [JsonIgnore]
        public Feature Entity { get; private set; }

        [CanBeNull]
        public readonly FeatureViewModel Instance;

        public FeatureViewModel()
        {
        }

        public FeatureViewModel(Feature entity, bool includeDeleted) : base(entity)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Instance = new FeatureViewModel
            {
                Entity = entity,
                Name = entity.Name,
                Type = entity.Type,
                Id = entity.Id,
                Properties = entity.PropertyFeatures?.Count ?? 0,
                Logs = entity.GetLogs()
            };
        }

        public string Name { get; set; }

        public FeatureTypeEnum Type { get; set; }

        public int Properties { get; set; }
    }
}