using System.Collections.Generic;
using RealEstate.Base;
using RealEstate.Base.Enums;

namespace RealEstate.ViewModels
{
    public class DealViewModel : BaseTrackViewModel
    {
        public ItemRequestStatusEnum Status { get; set; }
        public ItemViewModel Item { get; set; }
        public List<ApplicantViewModel> Applicant { get; set; }
        public List<DealPaymentViewModel> DealPayments { get; set; }
        public List<BeneficiaryViewModel> Beneficiaries { get; set; }
    }
}