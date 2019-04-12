using JetBrains.Annotations;
using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;

namespace RealEstate.Services.ViewModels
{
    public class PropertyFacilityViewModel : BaseLogViewModel<PropertyFacility>
    {
        [JsonIgnore]
        public PropertyFacility Entity { get; private set; }

        [CanBeNull]
        public readonly PropertyFacilityViewModel Instance;

        public PropertyFacilityViewModel(PropertyFacility entity, bool includeDeleted) : base(entity)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Instance = new PropertyFacilityViewModel
            {
                Entity = entity,
                Id = entity.Id,
                Logs = entity.GetLogs()
            };
        }

        public PropertyFacilityViewModel()
        {
        }

        public FacilityViewModel Facility { get; set; }
    }
}