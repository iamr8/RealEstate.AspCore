using RealEstate.Base;
using System.Collections.Generic;

namespace RealEstate.ViewModels
{
    public class DealViewModel : BaseLogViewModel
    {
        public ItemRequestViewModel ItemRequest { get; set; }
        public List<DealPaymentViewModel> DealPayments { get; set; }
        public List<BeneficiaryViewModel> Beneficiaries { get; set; }
        public List<PictureViewModel> Pictures { get; set; }
    }
}