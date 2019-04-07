using RealEstate.Base;

namespace RealEstate.ViewModels
{
    public class OwnershipViewModel : BaseLogViewModel
    {
        public string Name { get; set; }

        public string Phone { get; set; }
        public string Address { get; set; }

        public string Description { get; set; }
        public int Dong { get; set; }
        public ContactViewModel Contact { get; set; }
        public string PropertyOwnershipId { get; set; }
    }
}