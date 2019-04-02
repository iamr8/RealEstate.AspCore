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
        public string CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }
}