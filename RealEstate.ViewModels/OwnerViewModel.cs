using RealEstate.Base;

namespace RealEstate.ViewModels
{
    public class OwnerViewModel : BaseTrackViewModel
    {
        public int Dong { get; set; }
        public ContactViewModel Contact { get; set; }
        public string OwnershipId { get; set; }
    }
}