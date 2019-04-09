using RealEstate.Base;
using RealEstate.Domain.Tables;

namespace RealEstate.ViewModels
{
    public class OwnershipViewModel : BaseLogViewModel<Ownership>
    {
        public OwnershipViewModel()
        {
        }

        public OwnershipViewModel(Ownership entity) : base(entity)
        {
            if (entity == null)
                return;

            Id = Entity.Id;
            Name = Entity.Name;
            Phone = Entity.PhoneNumber;
            Address = Entity.Address;
            Description = Entity.Description;
            Dong = Entity.Dong;
        }

        public string Name { get; set; }

        public string Phone { get; set; }
        public string Address { get; set; }

        public string Description { get; set; }
        public int Dong { get; set; }
        public ContactViewModel Contact { get; set; }
        public PropertyViewModel Property { get; set; }
    }
}