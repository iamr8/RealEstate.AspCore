using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels
{
    public class ItemRequestViewModel : BaseLogViewModel<ItemRequest>
    {
        public bool IsReject { get; set; }
        public ItemViewModel Item { get; set; }
        public List<ApplicantViewModel> Applicants { get; set; }
    }
}