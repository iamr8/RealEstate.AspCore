using RealEstate.Services.Database.Base;

namespace RealEstate.Services.Database.Tables
{
    public class PropertyFacility : BaseEntity
    {
        public virtual Property Property { get; set; }

        public string PropertyId { get; set; }

        public virtual Facility Facility { get; set; }

        public string FacilityId { get; set; }

        public override string ToString()
        {
            return Facility.ToString();
        }
    }
}