using RealEstate.Base.Enums;
using RealEstate.Services.Database.Base;
using System;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.Database.Tables
{
    public class Presence : BaseEntity
    {
        [Required]
        public PresenseStatusEnum Status { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public int Hour { get; set; }

        public int Minute { get; set; }

        public string EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
    }
}