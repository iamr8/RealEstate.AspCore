using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.Base;
using RealEstate.Domain.Tables;

namespace RealEstate.Domain.Configuration
{
    internal class ItemFeatureConfig : BaseEntityConfiguration<ItemFeature>
    {
        public override void Configure(EntityTypeBuilder<ItemFeature> builder)
        {
            base.Configure(builder);
            builder.HasOne(x => x.Item)
                .WithMany(x => x.ItemFeatures)
                .HasForeignKey(x => x.ItemId)
                .IsRequired();

            builder.HasOne(x => x.Feature)
                .WithMany(x => x.ItemFeatures)
                .HasForeignKey(x => x.FeatureId)
                .IsRequired();
        }
    }
}