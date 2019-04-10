using RealEstate.Base.Enums;
using RealEstate.Services.Database.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.Database.Tables
{
    public class Category : BaseEntity
    {
        public Category()
        {
            Items = new HashSet<Item>();
            UserItemCategories = new HashSet<UserItemCategory>();
            Properties = new HashSet<Property>();
            UserPropertyCategories = new HashSet<UserPropertyCategory>();
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