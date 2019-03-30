using System.Collections.Generic;
using RealEstate.Base.Database;

namespace RealEstate.Domain.Tables
{
    public class Item : BaseEntity
    {
        public Item()
        {
            ItemFeatures = new HashSet<ItemFeature>();
            ItemRequests = new HashSet<ItemRequest>();
        }

        public string Description { get; set; }

        public string PropertyId { get; set; }

        public string CategoryId { get; set; }
        public virtual ItemCategory Category { get; set; }
        public virtual Property Property { get; set; }
        public virtual ICollection<ItemFeature> ItemFeatures { get; set; }
        public virtual ICollection<ItemRequest> ItemRequests { get; set; }
    }
}