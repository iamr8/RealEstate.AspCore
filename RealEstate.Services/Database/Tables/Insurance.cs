using RealEstate.Services.Database.Base;
using RealEstate.Services.Extensions;
using System.ComponentModel.DataAnnotations;
using RealEstate.Base;

namespace RealEstate.Services.Database.Tables
{
    public class Insurance : BaseEntity
    {
        [Required]
        public double Price { get; set; }

        public string EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }

        public override string ToString()
        {
            return Price.ToString().FixCurrency();
        }
    }
}