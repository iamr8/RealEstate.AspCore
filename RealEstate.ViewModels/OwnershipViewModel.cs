using RealEstate.Base;

namespace RealEstate.ViewModels
{
    public class OwnershipViewModel : BaseLogViewModel
    {
        public int Dong { get; set; }
        public ContactViewModel Contact { get; set; }
        public string PropertyOwnershipId { get; set; }
    }
}