using RealEstate.Services.Database.Base;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.Database.Tables
{
    public class FixedSalary : BaseEntity
    {
        [Required]
        public double Value { get; set; }

        public string EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
    }
}