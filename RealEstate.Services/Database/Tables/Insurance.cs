using RealEstate.Services.Database.Base;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.Database.Tables
{
    public class Insurance : BaseEntity
    {
        [Required]
        public double Price { get; set; }

        public string EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
    }
}