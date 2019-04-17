using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;

namespace RealEstate.Services.ViewModels
{
    public class BeneficiaryViewModel : BaseLogViewModel<Beneficiary>
    {
        [JsonIgnore]
        private readonly Beneficiary _entity;

        public BeneficiaryViewModel(Beneficiary entity, bool includeDeleted, Action<BeneficiaryViewModel> action = null) : base(entity)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            _entity = entity;
            Id = entity.Id;
            Logs = entity.GetLogs();
            action?.Invoke(this);
        }

        public int TipPercent => _entity.TipPercent;

        public int CommissionPercent => _entity.CommissionPercent;

        public void GetUser(bool includeDeleted = false, Action<UserViewModel> action = null)
        {
            User = _entity?.User.Into(includeDeleted, action);
        }

        public void GetDeal(bool includeDeleted = false, Action<DealViewModel> action = null)
        {
            Deal = _entity?.Deal.Into(includeDeleted, action);
        }

        public UserViewModel User { get; private set; }
        public DealViewModel Deal { get; private set; }
    }
}