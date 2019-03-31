using RealEstate.Domain.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Domain.Tables
{
    public class Beneficiary : BaseEntity
    {
        public Beneficiary()
        {
            Logs = new HashSet<Log>();
        }

        [Required]
        public int TipPercent { get; set; }

        [Required]
        public int CommissionPercent { get; set; }

        public string UserId { get; set; }

        public string DealId { get; set; }

        public virtual User User { get; set; }
        public virtual Deal Deal { get; set; }
    }
}