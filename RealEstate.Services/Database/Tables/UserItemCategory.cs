using RealEstate.Services.Database.Base;

namespace RealEstate.Services.Database.Tables
{
    public class UserItemCategory : BaseEntity
    {
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public string CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }
}