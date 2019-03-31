using System.Collections.Generic;
using RealEstate.Domain.Base;

namespace RealEstate.Domain.Tables
{
    public class UserPropertyCategory : BaseEntity
    {
        public UserPropertyCategory()
        {
            Logs = new HashSet<Log>();
        }

        public string UserId { get; set; }
        public virtual User User { get; set; }
        public string PropertyCategoryId { get; set; }
        public virtual PropertyCategory PropertyCategory { get; set; }
    }
}