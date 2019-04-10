using RealEstate.Domain.Tables;
using RealEstate.Services.BaseLog;

namespace RealEstate.Services.ViewModels
{
    public class BeneficiaryViewModel : BaseLogViewModel<Beneficiary>
    {
        protected BeneficiaryViewModel(Beneficiary entity, bool showDeleted) : base(entity)
        {
            if (entity == null)
                return;

            TipPercent = Entity.TipPercent;
            CommissionPercent = Entity.CommissionPercent;
            Id = Entity.Id;
        }

        public BeneficiaryViewModel()
        {
        }

        public int TipPercent { get; set; }

        public int CommissionPercent { get; set; }
        public UserViewModel User { get; set; }
    }
}