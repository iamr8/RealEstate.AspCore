using RealEstate.Base.Enums;
using RealEstate.Services.Database.Base;

namespace RealEstate.Services.Database.Tables
{
    public class DealRequest : BaseEntity
    {
        public DealStatusEnum Status { get; set; }

        public string ItemId { get; set; }
        public string DealId { get; set; }
        public virtual Item Item { get; set; }

        public virtual Deal Deal { get; set; }
    }
}