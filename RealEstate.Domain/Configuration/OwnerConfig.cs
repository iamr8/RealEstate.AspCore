using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Base.Database;
using RealEstate.Domain.Tables;

namespace RealEstate.Domain.Configuration
{
    public class OwnerConfig : BaseEntityConfiguration<Owner>
    {
        public override void Configure(EntityTypeBuilder<Owner> builder)
        {
            base.Configure(builder);
            builder.HasOne(x => x.Contact)
                .WithMany(x => x.Owners)
                .HasForeignKey(x => x.ContactId)
                .IsRequired();

            builder.HasOne(x => x.Ownership)
                .WithMany(x => x.Owners)
                .HasForeignKey(x => x.OwnershipId);
        }
    }
}