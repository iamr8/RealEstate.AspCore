using RealEstate.Services.Database.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.Database.Tables
{
    public class Check : BaseEntity
    {
        [Required]
        public DateTime PayDate { get; set; }

        [Required]
        public string Bank { get; set; }

        public string CheckNumber { get; set; }

        [Required]
        public string Price { get; set; }

        public string Description { get; set; }
        public string DealId { get; set; }
        public string ReminderId { get; set; }
        public virtual Deal Deal { get; set; }
        public virtual Reminder Reminder { get; set; }
        public virtual ICollection<Picture> Pictures { get; set; }
    }
}