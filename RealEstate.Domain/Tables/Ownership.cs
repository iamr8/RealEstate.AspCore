using System.ComponentModel;
using RealEstate.Base.Database;

namespace RealEstate.Domain.Tables
{
    public class Ownership : BaseEntity
    {
        [DefaultValue(6)]
        public int Dong { get; set; }

        public string PropertyOwnershipId { get; set; }
        public string ContactId { get; set; }
        public virtual PropertyOwnership PropertyOwnership { get; set; }
        public virtual Contact Contact { get; set; }
    }
}