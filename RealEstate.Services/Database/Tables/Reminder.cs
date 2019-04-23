using RealEstate.Services.Database.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.Database.Tables
{
    public class Reminder : BaseEntity
    {
        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public string UserId { get; set; }
        public virtual ICollection<Check> Checks { get; set; }
        public virtual User User { get; set; }
    }
}