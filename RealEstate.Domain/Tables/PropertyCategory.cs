using RealEstate.Domain.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Domain.Tables
{
    public class PropertyCategory : BaseEntity
    {
        public PropertyCategory()
        {
            Properties = new HashSet<Property>();
            UserPropertyCategories = new HashSet<UserPropertyCategory>();
            Logs = new HashSet<Log>();
        }

        [Required]
        public string Name { get; set; }

        public virtual ICollection<Property> Properties { get; set; }
        public virtual ICollection<UserPropertyCategory> UserPropertyCategories { get; set; }
    }
}