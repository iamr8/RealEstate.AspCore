using RealEstate.Base.Enums;
using RealEstate.Services.Database.Base;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.Database.Tables
{
    public class EmployeeStatus : BaseEntity
    {
        [Required]
        public EmployeeStatusEnum Status { get; set; }

        public string EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
    }
}