using System.Collections.Generic;
using RealEstate.Base.Database;

namespace RealEstate.Domain.Tables
{
    public class PropertyOwnership : BaseEntity
    {
        public PropertyOwnership()
        {
            Ownerships = new HashSet<Ownership>();
        }

        public string PropertyId { get; set; }

        public virtual Property Property { get; set; }
        public virtual ICollection<Ownership> Ownerships { get; set; }
    }
}