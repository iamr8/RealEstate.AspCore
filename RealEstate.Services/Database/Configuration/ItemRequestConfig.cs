using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Services.Database.Base;
using RealEstate.Services.Database.Tables;

namespace RealEstate.Services.Database.Configuration
{
    public class ItemRequestConfig : BaseEntityConfiguration<ItemRequest>
    {
        public override void Configure(EntityTypeBuilder<ItemRequest> builder)
        {
            base.Configure(builder);
            builder.HasOne(x => x.Item)
                .WithMany(x => x.ItemRequests)
                .HasForeignKey(x => x.ItemId)
                .IsRequired();

            builder.HasOne(x => x.Deal)
                .WithOne(x => x.ItemRequest)
                .HasForeignKey<Deal>(x => x.ItemRequestId);

            builder.HasMany(x => x.Applicants)
                .WithOne(x => x.ItemRequest)
                .HasForeignKey(x => x.ItemRequestId);
        }
    }
}