using RealEstate.Services.Database.Base;
using System;
using System.Collections.Generic;

namespace RealEstate.Services.Database.Tables
{
    public class Reminder : BaseEntity
    {
        public string Text { get; set; }
        public DateTime AlarmTime { get; set; }
        public string UserId { get; set; }
        public virtual ICollection<Check> Checks { get; set; }
        public virtual User User { get; set; }
    }
}