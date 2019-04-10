using RealEstate.Services.Database.Base;

namespace RealEstate.Services.Database.Tables
{
    public class FixedSalary : BaseEntity
    {
        public double Value { get; set; }

        public string UserId { get; set; }
        public virtual User User { get; set; }
    }
}