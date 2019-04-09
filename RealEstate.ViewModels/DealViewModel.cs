using RealEstate.Base;
using RealEstate.Domain.Tables;
using System.Collections.Generic;

namespace RealEstate.ViewModels
{
    public class DealViewModel : BaseLogViewModel<Deal>
    {
        public ItemRequestViewModel ItemRequest { get; set; }
        public List<DealPaymentViewModel> DealPayments { get; set; }
        public List<BeneficiaryViewModel> Beneficiaries { get; set; }
        public List<PictureViewModel> Pictures { get; set; }
    }
}