using RealEstate.Services.Database.Base;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.Database.Tables
{
    public class Beneficiary : BaseEntity
    {
        [Required]
        public int TipPercent { get; set; }

        [Required]
        public int CommissionPercent { get; set; }

        public string UserId { get; set; }

        public string DealId { get; set; }
        public virtual User User { get; set; }
        public virtual Deal Deal { get; set; }

        public override string ToString()
        {
            return $"شیرینی : {TipPercent}% | کمیسیون : {CommissionPercent}%";
        }
    }
}