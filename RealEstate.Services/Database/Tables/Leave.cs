using RealEstate.Services.Database.Base;
using System;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.Database.Tables
{
    public class Leave : BaseEntity
    {
        [Required]
        public DateTime From { get; set; }

        [Required]
        public DateTime To { get; set; }

        public string Reason { get; set; }
        public string EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
    }
}