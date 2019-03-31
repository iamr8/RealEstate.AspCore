using RealEstate.Domain.Base;
using System;
using System.Collections.Generic;

namespace RealEstate.Domain.Tables
{
    public class DealPayment : BaseEntity
    {
        public DealPayment()
        {
            Pictures = new HashSet<Picture>();
            Logs = new HashSet<Log>();
        }

        public decimal CommissionPrice { get; set; } // مبلغ کمیسیون
        public decimal TipPrice { get; set; } // مبلغ شیرینی
        public DateTime PayDate { get; set; }
        public string Text { get; set; }
        public string DealId { get; set; }
        public virtual Deal Deal { get; set; }
        public virtual ICollection<Picture> Pictures { get; set; }
    }
}