using RealEstate.Services.Database.Base;
using System.Collections.Generic;

namespace RealEstate.Services.Database.Tables
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