using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;

namespace RealEstate.Services.ViewModels
{
    public class BeneficiaryViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public Beneficiary Entity { get; }

        public BeneficiaryViewModel(Beneficiary entity, bool includeDeleted, Action<BeneficiaryViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Entity = entity;
            action?.Invoke(this);
        }

        public int TipPercent => Entity.TipPercent;

        public int CommissionPercent => Entity.CommissionPercent;

        public void GetUser(bool includeDeleted = false, Action<UserViewModel> action = null)
        {
            User = Entity?.User.Into(includeDeleted, action);
        }

        public void GetDeal(bool includeDeleted = false, Action<DealViewModel> action = null)
        {
            Deal = Entity?.Deal.Into(includeDeleted, action);
        }

        public UserViewModel User { get; private set; }
        public DealViewModel Deal { get; private set; }
    }
}