using RealEstate.Base.Database;

namespace RealEstate.Domain.Tables
{
    public class UserPropertyCategory : BaseEntity
    {
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public string PropertyCategoryId { get; set; }
        public virtual PropertyCategory PropertyCategory { get; set; }
    }
}