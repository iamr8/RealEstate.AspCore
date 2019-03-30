using System.ComponentModel.DataAnnotations;
using RealEstate.Base.Database;

namespace RealEstate.Domain.Tables
{
    public class Picture : BaseEntity
    {
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