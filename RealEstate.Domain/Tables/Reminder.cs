using RealEstate.Domain.Base;
using System;
using System.Collections.Generic;

namespace RealEstate.Domain.Tables
{
    public class Reminder : BaseEntity
    {
        public Reminder()
        {
            Logs = new HashSet<Log>();
        }

        public string UserId { get; set; }
        public string Text { get; set; }
        public DateTime AlarmTime { get; set; }
        public virtual User User { get; set; }
    }
}