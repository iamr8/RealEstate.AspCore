using RealEstate.Services.Database.Base;

namespace RealEstate.Services.Database.Tables
{
    public class FixedSalary : BaseEntity
    {
        public double Value { get; set; }

        public string EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
    }
}