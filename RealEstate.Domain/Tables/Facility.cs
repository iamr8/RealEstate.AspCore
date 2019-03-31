using RealEstate.Domain.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Domain.Tables
{
    public class Facility : BaseEntity
    {
        public Facility()
        {
            PropertyFacilities = new HashSet<PropertyFacility>();
            Logs = new HashSet<Log>();
        }

        [Required]
        public string Name { get; set; }

        public virtual ICollection<PropertyFacility> PropertyFacilities { get; set; }
    }
}