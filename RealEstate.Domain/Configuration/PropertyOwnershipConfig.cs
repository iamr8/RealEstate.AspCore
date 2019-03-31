using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.Base;
using RealEstate.Domain.Tables;

namespace RealEstate.Domain.Configuration
{
    public class PropertyOwnershipConfig : BaseEntityConfiguration<PropertyOwnership>
    {
        public override void Configure(EntityTypeBuilder<PropertyOwnership> builder)
        {
            base.Configure(builder);
            builder.HasOne(x => x.Property)
                .WithMany(x => x.PropertyOwnerships)
                .HasForeignKey(x => x.PropertyId)
                .IsRequired();
        }
    }
}