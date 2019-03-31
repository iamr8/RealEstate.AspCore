using RealEstate.Base.Enums;
using RealEstate.Domain.Base;
using System.Collections.Generic;

namespace RealEstate.Domain.Tables
{
    public class Payment : BaseEntity
    {
        public Payment()
        {
            Pictures = new HashSet<Picture>();
            Logs = new HashSet<Log>();
        }

        // Calculation Method : Sum(Salary) + Sum(Gift) - Sum(Forfeit) - Sum(Advance)
        public double Value { get; set; }

        public string Text { get; set; }
        public PaymentTypeEnum Type { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Picture> Pictures { get; set; }
    }
}