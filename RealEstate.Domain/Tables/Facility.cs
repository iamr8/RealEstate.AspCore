using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RealEstate.Base.Database;

namespace RealEstate.Domain.Tables
{
    public class Facility : BaseEntity
    {
        public Facility()
        {
            PropertyFacilities = new HashSet<PropertyFacility>();
        }

        [Required]
        public string Name { get; set; }

        public virtual ICollection<PropertyFacility> PropertyFacilities { get; set; }
    }
}