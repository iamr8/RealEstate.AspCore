using RealEstate.Services.Database.Base;
using System;

namespace RealEstate.Services.Database.Tables
{
    public class Reminder : BaseEntity
    {
        public string UserId { get; set; }
        public string Text { get; set; }
        public DateTime AlarmTime { get; set; }
        public virtual User User { get; set; }
    }
}