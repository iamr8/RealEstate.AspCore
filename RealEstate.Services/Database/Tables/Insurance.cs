using RealEstate.Base.Enums;
using RealEstate.Services.Database.Base;

namespace RealEstate.Services.Database.Tables
{
    public class Insurance : BaseEntity
    {
        public string Price { get; set; }
        public string Code { get; set; }
        public InsuranceStatusEnum Status { get; set; }
        public string EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
    }
}