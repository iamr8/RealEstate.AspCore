using JetBrains.Annotations;
using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;

namespace RealEstate.Services.ViewModels
{
    public class OwnershipViewModel : BaseLogViewModel<Ownership>
    {
        public OwnershipViewModel()
        {
        }

        [JsonIgnore]
        public Ownership Entity { get; private set; }

        [CanBeNull]
        public readonly OwnershipViewModel Instance;

        public OwnershipViewModel(Ownership entity, bool includeDeleted) : base(entity)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Instance = new OwnershipViewModel
            {
                Entity = entity,
                Id = entity.Id,
                Description = entity.Description,
                Dong = entity.Dong,
                Logs = entity.GetLogs()
            };
        }

        public string Description { get; set; }
        public int Dong { get; set; }
        public ContactViewModel Contact { get; set; }
        public PropertyViewModel Property { get; set; }
    }
}