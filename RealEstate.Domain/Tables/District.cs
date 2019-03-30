using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RealEstate.Base.Database;

namespace RealEstate.Domain.Tables
{
    public class District : BaseEntity
    {
        public District()
        {
            Properties = new HashSet<Property>();
        }

        [Required]
        public string Name { get; set; }

        public virtual ICollection<Property> Properties { get; set; }
    }
}