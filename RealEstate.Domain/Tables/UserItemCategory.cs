using RealEstate.Domain.Base;
using System.Collections.Generic;

namespace RealEstate.Domain.Tables
{
    public class UserItemCategory : BaseEntity
    {
        public UserItemCategory()
        {
            Logs = new HashSet<Log>();
        }

        public string UserId { get; set; }
        public virtual User User { get; set; }
        public string ItemCategoryId { get; set; }
        public virtual ItemCategory ItemCategory { get; set; }
    }
}