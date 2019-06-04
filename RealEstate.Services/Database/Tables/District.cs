using RealEstate.Services.Database.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.Database.Tables
{
    public class District : BaseEntity
    {
        [Required]
        public string Name { get; set; }

        public virtual ICollection<Property> Properties { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}