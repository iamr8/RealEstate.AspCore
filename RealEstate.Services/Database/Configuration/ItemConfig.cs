using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Services.Database.Base;
using RealEstate.Services.Database.Tables;

namespace RealEstate.Services.Database.Configuration
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

            builder.HasMany(x => x.Applicants)
                .WithOne(x => x.Item)
                .HasForeignKey(x => x.ItemId);
        }
    }
}