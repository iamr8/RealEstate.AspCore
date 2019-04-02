using RealEstate.Base.Enums;
using RealEstate.Domain.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Domain.Tables
{
    public class Category : BaseEntity
    {
        public Category()
        {
            Items = new HashSet<Item>();
            UserItemCategories = new HashSet<UserItemCategory>();
            Properties = new HashSet<Property>();
            UserPropertyCategories = new HashSet<UserPropertyCategory>();
            Logs = new HashSet<Log>();
        }

        [Required]
        public string Name { get; set; }

        public CategoryTypeEnum Type { get; set; }

        public virtual ICollection<Item> Items { get; set; }
        public virtual ICollection<Property> Properties { get; set; }
        public virtual ICollection<UserItemCategory> UserItemCategories { get; set; }
        public virtual ICollection<UserPropertyCategory> UserPropertyCategories { get; set; }
    }
}