using RealEstate.Base.Database;

namespace RealEstate.Domain.Tables
{
    public class NotificationRecipient : BaseEntity
    {
        public string LogId { get; set; }
        public virtual Log Log { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }
    }
}