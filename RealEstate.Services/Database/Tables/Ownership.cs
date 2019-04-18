using RealEstate.Services.Database.Base;
using System.ComponentModel;

namespace RealEstate.Services.Database.Tables
{
    public class Ownership : BaseEntity
    {
        [DefaultValue(6)]
        public int Dong { get; set; }
        public string Description { get; set; }
        public string PropertyOwnershipId { get; set; }
        public string CustomerId { get; set; }
        public virtual PropertyOwnership PropertyOwnership { get; set; }
        public virtual Customer Customer { get; set; }
    }
}