using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.Base;
using RealEstate.Domain.Tables;

namespace RealEstate.Domain.Configuration
{
    internal class CategoryConfig : BaseEntityConfiguration<Category>
    {
        public override void Configure(EntityTypeBuilder<Category> builder)
        {
            base.Configure(builder);

            builder.HasMany(x => x.Properties)
                .WithOne(x => x.Category)
                .HasForeignKey(x => x.CategoryId);
        }
    }
}