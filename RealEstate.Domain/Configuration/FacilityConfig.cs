using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Base.Database;
using RealEstate.Domain.Tables;

namespace RealEstate.Domain.Configuration
{
    public class FacilityConfig : BaseEntityConfiguration<Facility>
    {
        public override void Configure(EntityTypeBuilder<Facility> builder)
        {
            base.Configure(builder);
        }
    }
}