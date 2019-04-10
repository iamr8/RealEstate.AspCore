using RealEstate.Services.Database.Base;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.Database.Tables
{
    public class Ownership : BaseEntity
    {
        [DefaultValue(6)]
        public int Dong { get; set; }

        [Required]
        public string Name { get; set; }

        public string PhoneNumber { get; set; }
        public string Address { get; set; }

        public string Description { get; set; }
        public string PropertyOwnershipId { get; set; }
        public string ContactId { get; set; }
        public virtual PropertyOwnership PropertyOwnership { get; set; }
        public virtual Contact Contact { get; set; }
    }
}