using RealEstate.Services.Database.Base;

namespace RealEstate.Services.Database.Tables
{
    public class ManagementPercent : BaseEntity
    {
        public int Percent { get; set; }
        public string EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
    }
}