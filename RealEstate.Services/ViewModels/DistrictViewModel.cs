using JetBrains.Annotations;
using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;

namespace RealEstate.Services.ViewModels
{
    public class DistrictViewModel : BaseLogViewModel<District>
    {
        [JsonIgnore]
        public District Entity { get; private set; }

        [CanBeNull]
        public readonly DistrictViewModel Instance;

        public DistrictViewModel(District entity, bool includeDeleted) : base(entity)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Instance = new DistrictViewModel
            {
                Entity = entity,
                Id = entity.Id,
                Name = entity.Name,
                Properties = entity.Properties?.Count ?? 0,
                Logs = entity.GetLogs()
            };
        }

        public DistrictViewModel()
        {
        }

        public string Name { get; set; }

        public int Properties { get; set; }
    }
}