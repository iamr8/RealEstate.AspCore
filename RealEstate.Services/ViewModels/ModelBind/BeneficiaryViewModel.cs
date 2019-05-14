using System;
using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class BeneficiaryViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public Beneficiary Entity { get; }

        public BeneficiaryViewModel(Beneficiary entity)
        {
            if (entity == null)
                return;

            Entity = entity;
        }

        public int TipPercent => Entity.TipPercent;

        public int CommissionPercent => Entity.CommissionPercent;

        public Lazy<UserViewModel> User => LazyLoadExtension.LazyLoad(() => Entity?.User.Into<User, UserViewModel>());
        public Lazy<DealViewModel> Deal => LazyLoadExtension.LazyLoad(() => Entity?.Deal.Into<Deal, DealViewModel>());
    }
}