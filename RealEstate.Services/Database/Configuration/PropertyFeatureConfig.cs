using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Services.Database.Base;
using RealEstate.Services.Database.Tables;

namespace RealEstate.Services.Database.Configuration
{
    public class PropertyFeatureConfig : BaseEntityConfiguration<PropertyFeature>
    {
        public override void Configure(EntityTypeBuilder<PropertyFeature> builder)
        {
            base.Configure(builder);
            builder.HasOne(x => x.Property)
                .WithMany(x => x.PropertyFeatures)
                .HasForeignKey(x => x.PropertyId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Feature)
                .WithMany(x => x.PropertyFeatures)
                .HasForeignKey(x => x.FeatureId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}