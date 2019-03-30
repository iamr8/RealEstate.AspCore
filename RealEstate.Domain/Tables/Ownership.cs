using System.Collections.Generic;
using RealEstate.Base.Database;

namespace RealEstate.Domain.Tables
{
    public class Ownership : BaseEntity
    {
        public Ownership()
        {
            Owners = new HashSet<Owner>();
        }

        public string PropertyId { get; set; }

        public virtual Property Property { get; set; }
        public virtual ICollection<Owner> Owners { get; set; }
    }
}