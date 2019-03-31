using RealEstate.Domain.Base;
using System.Collections.Generic;

namespace RealEstate.Domain.Tables
{
    public class PropertyFacility : BaseEntity
    {
        public PropertyFacility()
        {
            Logs = new HashSet<Log>();
        }

        public virtual Property Property { get; set; }

        public string PropertyId { get; set; }

        public virtual Facility Facility { get; set; }

        public string FacilityId { get; set; }
    }
}