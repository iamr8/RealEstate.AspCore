using RealEstate.Domain.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Domain.Tables
{
    public class ItemCategory : BaseEntity
    {
        public ItemCategory()
        {
            Items = new HashSet<Item>();
            UserItemCategories = new HashSet<UserItemCategory>();
            Logs = new HashSet<Log>();
        }

        [Required]
        public string Name { get; set; }

        public virtual ICollection<Item> Items { get; set; }
        public virtual ICollection<UserItemCategory> UserItemCategories { get; set; }
    }
}