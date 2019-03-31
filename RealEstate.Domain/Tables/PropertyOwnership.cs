using RealEstate.Domain.Base;
using System.Collections.Generic;

namespace RealEstate.Domain.Tables
{
    public class PropertyOwnership : BaseEntity
    {
        public PropertyOwnership()
        {
            Ownerships = new HashSet<Ownership>();
            Logs = new HashSet<Log>();
        }

        public string PropertyId { get; set; }

        public virtual Property Property { get; set; }
        public virtual ICollection<Ownership> Ownerships { get; set; }
    }
}