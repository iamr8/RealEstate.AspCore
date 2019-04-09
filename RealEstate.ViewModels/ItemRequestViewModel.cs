using RealEstate.Base;
using RealEstate.Domain.Tables;
using System.Collections.Generic;

namespace RealEstate.ViewModels
{
    public class ItemRequestViewModel : BaseLogViewModel<ItemRequest>
    {
        public bool IsReject { get; set; }
        public ItemViewModel Item { get; set; }
        public List<ApplicantViewModel> Applicants { get; set; }
    }
}