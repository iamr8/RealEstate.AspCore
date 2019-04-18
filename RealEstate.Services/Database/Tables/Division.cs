using RealEstate.Services.Database.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.Database.Tables
{
    public class Division : BaseEntity
    {
        [Required]
        public string Subject { get; set; }

        public virtual ICollection<EmployeeDivision> EmployeeDivisions { get; set; }
    }
}