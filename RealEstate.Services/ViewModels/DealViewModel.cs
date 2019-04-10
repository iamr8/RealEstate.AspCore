using RealEstate.Domain.Tables;
using RealEstate.Services.BaseLog;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels
{
    public class DealViewModel : BaseLogViewModel<Deal>
    {
        public ItemRequestViewModel ItemRequest { get; set; }
        public List<DealPaymentViewModel> DealPayments { get; set; }
        public List<BeneficiaryViewModel> Beneficiaries { get; set; }
        public List<PictureViewModel> Pictures { get; set; }
    }
}