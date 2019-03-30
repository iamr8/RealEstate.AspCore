using System.ComponentModel;
using RealEstate.Base.Database;

namespace RealEstate.Domain.Tables
{
    public class Owner : BaseEntity
    {
        [DefaultValue(6)]
        public int Dong { get; set; }

        public string OwnershipId { get; set; }
        public string ContactId { get; set; }
        public virtual Ownership Ownership { get; set; }
        public virtual Contact Contact { get; set; }
    }
}