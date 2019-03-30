using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Base.Database;
using RealEstate.Domain.Tables;

namespace RealEstate.Domain.Configuration
{
    public class OwnershipConfig : BaseEntityConfiguration<Ownership>
    {
        public override void Configure(EntityTypeBuilder<Ownership> builder)
        {
            base.Configure(builder);
            builder.HasOne(x => x.Property)
                .WithMany(x => x.Ownerships)
                .HasForeignKey(x => x.PropertyId)
                .IsRequired();
        }
    }
}