using RealEstate.Domain.Base;
using System.Collections.Generic;
using System.ComponentModel;

namespace RealEstate.Domain.Tables
{
    public class Ownership : BaseEntity
    {
        public Ownership()
        {
            Logs = new HashSet<Log>();
        }

        [DefaultValue(6)]
        public int Dong { get; set; }

        public string PropertyOwnershipId { get; set; }
        public string ContactId { get; set; }
        public virtual PropertyOwnership PropertyOwnership { get; set; }
        public virtual Contact Contact { get; set; }
    }
}