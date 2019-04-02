using RealEstate.Base;
using System.Collections.Generic;

namespace RealEstate.ViewModels
{
    public class ItemRequestViewModel : BaseLogViewModel
    {
        public bool IsReject { get; set; }
        public ItemViewModel Item { get; set; }
        public List<ApplicantViewModel> Applicants { get; set; }
    }
}