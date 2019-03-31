using RealEstate.Domain.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Domain.Tables
{
    public class Picture : BaseEntity
    {
        public Picture()
        {
            Logs = new HashSet<Log>();
        }

        [Required]
        public string File { get; set; }

        public string Text { get; set; }
        public string PropertyId { get; set; }
        public virtual Property Property { get; set; }
        public string PaymentId { get; set; }
        public virtual Payment Payment { get; set; }
        public string DealId { get; set; }
        public virtual Deal Deal { get; set; }
        public string DealPaymentId { get; set; }
        public virtual DealPayment DealPayment { get; set; }
    }
}