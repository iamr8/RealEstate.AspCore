using RealEstate.Base.Enums;
using RealEstate.Services.Database.Base;
using System.Collections.Generic;

namespace RealEstate.Services.Database.Tables
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
        public string SmsId { get; set; }
        public string CheckoutId { get; set; }
        public string EmployeeId { get; set; }
        public virtual Payment Checkout { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Sms Sms { get; set; }
        public virtual ICollection<Picture> Pictures { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
    }
}