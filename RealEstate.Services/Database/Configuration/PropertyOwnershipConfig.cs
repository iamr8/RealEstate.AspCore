using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Services.Database.Base;
using RealEstate.Services.Database.Tables;

namespace RealEstate.Services.Database.Configuration
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