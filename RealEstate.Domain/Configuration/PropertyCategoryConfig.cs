using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Base.Database;
using RealEstate.Domain.Tables;

namespace RealEstate.Domain.Configuration
{
    public class PropertyCategoryConfig : BaseEntityConfiguration<PropertyCategory>
    {
        public override void Configure(EntityTypeBuilder<PropertyCategory> builder)
        {
            base.Configure(builder);
            builder.HasMany(x => x.Properties)
                .WithOne(x => x.PropertyCategory)
                .HasForeignKey(x => x.PropertyCategoryId)
                .IsRequired();
        }
    }
}