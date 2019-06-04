using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using System;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class BeneficiaryViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public Beneficiary Entity { get; }

        public BeneficiaryViewModel(Beneficiary entity, Action<BeneficiaryViewModel> act = null)
        {
            if (entity == null)
                return;

            Entity = entity;
            act?.Invoke(this);
        }

        public int TipPercent => Entity.TipPercent;

        public int CommissionPercent => Entity.CommissionPercent;

        public UserViewModel User { get; set; }
        public DealViewModel Deal { get; set; }

        public override string ToString()
        {
            return Entity.ToString();
        }
    }
}