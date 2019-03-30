using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RealEstate.Base.Database;

namespace RealEstate.Domain.Tables
{
    public class PropertyCategory : BaseEntity
    {
        public PropertyCategory()
        {
            Properties = new HashSet<Property>();
            UserPropertyCategories = new HashSet<UserPropertyCategory>();
        }

        [Required]
        public string Name { get; set; }

        public virtual ICollection<Property> Properties { get; set; }
        public virtual ICollection<UserPropertyCategory> UserPropertyCategories { get; set; }
    }
}