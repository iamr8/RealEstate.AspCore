using JetBrains.Annotations;
using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;

namespace RealEstate.Services.ViewModels
{
    public class FacilityViewModel : BaseLogViewModel<Facility>
    {
        [JsonIgnore]
        public Facility Entity { get; private set; }

        [CanBeNull]
        public readonly FacilityViewModel Instance;

        public FacilityViewModel(Facility entity, bool includeDeleted) : base(entity)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Instance = new FacilityViewModel
            {
                Entity = entity,
                Name = entity.Name,
                Properties = entity.PropertyFacilities?.Count ?? 0,
                Id = entity.Id,
                Logs = entity.GetLogs()
            };
        }

        public FacilityViewModel()
        {
        }

        public string Name { get; set; }

        public int Properties { get; set; }
    }
}