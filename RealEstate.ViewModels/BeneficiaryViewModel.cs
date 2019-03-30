using RealEstate.Base;

namespace RealEstate.ViewModels
{
    public class BeneficiaryViewModel : BaseTrackViewModel
    {
        public int TipPercent { get; set; }

        public int CommissionPercent { get; set; }
        public UserViewModel User { get; set; }
    }
}