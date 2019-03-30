using System.Collections.Generic;
using RealEstate.Base;
using RealEstate.Base.Database;
using RealEstate.Base.Enums;

namespace RealEstate.Domain.Tables
{
    public class Payment : BaseEntity
    {
        public Payment()
        {
            Pictures = new HashSet<Picture>();
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