using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.Base;
using RealEstate.Domain.Tables;

namespace RealEstate.Domain.Configuration
{
    public class PropertyFacilityConfig : BaseEntityConfiguration<PropertyFacility>
    {
        public override void Configure(EntityTypeBuilder<PropertyFacility> builder)
        {
            base.Configure(builder);
            builder.HasOne(x => x.Facility)
                .WithMany(x => x.PropertyFacilities)
                .HasForeignKey(x => x.FacilityId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Property)
                .WithMany(x => x.PropertyFacilities)
                .HasForeignKey(x => x.PropertyId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}