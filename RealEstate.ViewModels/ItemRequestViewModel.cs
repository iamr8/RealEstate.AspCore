using RealEstate.Base;
using RealEstate.Base.Enums;

namespace RealEstate.ViewModels
{
    public class ItemRequestViewModel : BaseTrackViewModel
    {
        public ItemRequestStatusEnum Status { get; set; }
        public ItemViewModel Item { get; set; }
        public ContactViewModel SecondContact { get; set; }
    }
}