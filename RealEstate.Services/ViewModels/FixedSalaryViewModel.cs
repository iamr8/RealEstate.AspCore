using JetBrains.Annotations;
using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;

namespace RealEstate.Services.ViewModels
{
    public class FixedSalaryViewModel : BaseLogViewModel<FixedSalary>
    {
        [JsonIgnore]
        public FixedSalary Entity { get; private set; }

        [CanBeNull]
        public readonly FixedSalaryViewModel Instance;

        public FixedSalaryViewModel(FixedSalary entity, bool includeDeleted) : base(entity)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Instance = new FixedSalaryViewModel
            {
                Entity = entity,
                Value = entity.Value,
                Id = entity.Id,
                Logs = entity.GetLogs()
            };
        }

        public FixedSalaryViewModel()
        {
        }

        public double Value { get; set; }
        public UserViewModel User { get; set; }
    }
}