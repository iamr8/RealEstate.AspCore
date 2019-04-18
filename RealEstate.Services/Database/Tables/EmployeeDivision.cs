using RealEstate.Services.Database.Base;

namespace RealEstate.Services.Database.Tables
{
    public class EmployeeDivision : BaseEntity
    {
        public string EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
        public string DivisionId { get; set; }
        public virtual Division Division { get; set; }
    }
}