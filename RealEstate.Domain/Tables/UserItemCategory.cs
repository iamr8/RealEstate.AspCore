using RealEstate.Base.Database;

namespace RealEstate.Domain.Tables
{
    public class UserItemCategory : BaseEntity
    {
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public string ItemCategoryId { get; set; }
        public virtual ItemCategory ItemCategory { get; set; }
    }
}