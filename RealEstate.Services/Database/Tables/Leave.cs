using RealEstate.Services.Database.Base;
using System;

namespace RealEstate.Services.Database.Tables
{
    public class Leave : BaseEntity
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string Reason { get; set; }
        public string EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
    }
}