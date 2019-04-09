using RealEstate.Base;
using RealEstate.Domain.Tables;

namespace RealEstate.ViewModels
{
    public class BeneficiaryViewModel : BaseLogViewModel<Beneficiary>
    {
        protected BeneficiaryViewModel(Beneficiary entity) : base(entity)
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