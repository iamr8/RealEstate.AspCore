using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Services.Database.Base;
using RealEstate.Services.Database.Tables;

namespace RealEstate.Services.Database.Configuration
{
    public class DealRequestConfig : BaseEntityConfiguration<DealRequest>
    {
        public override void Configure(EntityTypeBuilder<DealRequest> builder)
        {
            base.Configure(builder);

            builder.HasOne(x => x.Deal)
                .WithOne(x => x.DealRequest)
                .HasForeignKey<DealRequest>(x => x.DealId);

            builder.HasOne(x => x.Item)
                .WithMany(x => x.DealRequests)
                .HasForeignKey(x => x.ItemId)
                .IsRequired();
        }
    }
}