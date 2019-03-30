using RealEstate.Base;

namespace RealEstate.ViewModels
{
    public class OwnershipViewModel : BaseTrackViewModel
    {
        public int Dong { get; set; }
        public ContactViewModel Contact { get; set; }
        public string PropertyOwnershipId { get; set; }
    }
}