using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Services.Database.Base;
using RealEstate.Services.Database.Tables;

namespace RealEstate.Services.Database.Configuration
{
    public class PictureConfig : BaseEntityConfiguration<Picture>
    {
        public override void Configure(EntityTypeBuilder<Picture> builder)
        {
            base.Configure(builder);
            builder.HasOne(x => x.Property)
                .WithMany(x => x.Pictures)
                .HasForeignKey(x => x.PropertyId);

            builder.HasOne(x => x.Payment)
                .WithMany(x => x.Pictures)
                .HasForeignKey(x => x.PaymentId);

            builder.HasOne(x => x.Deal)
                .WithMany(x => x.Pictures)
                .HasForeignKey(x => x.DealId);
        }
    }
}