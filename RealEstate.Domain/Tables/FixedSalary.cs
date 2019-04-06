using RealEstate.Domain.Base;
using System.Collections.Generic;

namespace RealEstate.Domain.Tables
{
    public class FixedSalary : BaseEntity
    {
        public FixedSalary()
        {
            Logs = new HashSet<Log>();
        }

        public double Value { get; set; }

        public string UserId { get; set; }
        public virtual User User { get; set; }
    }
}