using RealEstate.Services.Database.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.Database.Tables
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