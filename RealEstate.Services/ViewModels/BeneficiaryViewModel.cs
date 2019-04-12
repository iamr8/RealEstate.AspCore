using JetBrains.Annotations;
using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;

namespace RealEstate.Services.ViewModels
{
    public class BeneficiaryViewModel : BaseLogViewModel<Beneficiary>
    {
        [JsonIgnore]
        public Beneficiary Entity { get; private set; }

        [CanBeNull]
        public readonly BeneficiaryViewModel Instance;

        public BeneficiaryViewModel(Beneficiary entity, bool includeDeleted) : base(entity)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Instance = new BeneficiaryViewModel
            {
                Entity = entity,
                TipPercent = entity.TipPercent,
                CommissionPercent = entity.CommissionPercent,
                Id = entity.Id,
                Logs = entity.GetLogs()
            };
        }

        public BeneficiaryViewModel()
        {
        }

        public int TipPercent { get; set; }

        public int CommissionPercent { get; set; }
        public UserViewModel User { get; set; }
    }
}