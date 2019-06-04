using RealEstate.Services.Database.Base;
using RealEstate.Services.Extensions;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.Database.Tables
{
    public class FixedSalary : BaseEntity
    {
        [Required]
        public double Value { get; set; }

        public string EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }

        public override string ToString()
        {
            return Value.ToString().CurrencyToWords();
        }
    }
}