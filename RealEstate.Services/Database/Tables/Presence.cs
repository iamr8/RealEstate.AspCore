using RealEstate.Base.Enums;
using RealEstate.Services.Database.Base;

namespace RealEstate.Services.Database.Tables
{
    public class Presence : BaseEntity
    {
        public PresenseStatusEnum Status { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }

        public string EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
    }
}