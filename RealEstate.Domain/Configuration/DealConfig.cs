using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Base.Database;
using RealEstate.Domain.Tables;

namespace RealEstate.Domain.Configuration
{
    public class DealConfig : BaseEntityConfiguration<Deal>
    {
        public override void Configure(EntityTypeBuilder<Deal> builder)
        {
            base.Configure(builder);

            builder.HasMany(x => x.Beneficiaries)
                .WithOne(x => x.Deal)
                .HasForeignKey(x => x.DealId)
                .IsRequired();
        }
    }
}