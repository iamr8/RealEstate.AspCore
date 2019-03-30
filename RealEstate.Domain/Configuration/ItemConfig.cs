using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Base.Database;
using RealEstate.Domain.Tables;

namespace RealEstate.Domain.Configuration
{
    public class ItemConfig : BaseEntityConfiguration<Item>
    {
        public override void Configure(EntityTypeBuilder<Item> builder)
        {
            base.Configure(builder);
            builder.HasOne(x => x.Property)
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.PropertyId)
                .IsRequired();

            builder.HasOne(x => x.Category)
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.CategoryId)
                .IsRequired();
        }
    }
}