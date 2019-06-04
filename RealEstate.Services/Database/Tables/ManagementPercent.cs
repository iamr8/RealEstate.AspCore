using RealEstate.Services.Database.Base;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.Database.Tables
{
    public class ManagementPercent : BaseEntity
    {
        [Required]
        public int Percent { get; set; }

        public string EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }

        public override string ToString()
        {
            return $"{Percent}%";
        }
    }
}