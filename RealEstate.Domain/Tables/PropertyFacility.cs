using RealEstate.Base.Database;

namespace RealEstate.Domain.Tables
{
    public class PropertyFacility : BaseEntity
    {
        public virtual Property Property { get; set; }

        public string PropertyId { get; set; }

        public virtual Facility Facility { get; set; }

        public string FacilityId { get; set; }
    }
}