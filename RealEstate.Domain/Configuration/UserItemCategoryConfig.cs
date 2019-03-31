using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.Base;
using RealEstate.Domain.Tables;

namespace RealEstate.Domain.Configuration
{
    public class UserItemCategoryConfig : BaseEntityConfiguration<UserItemCategory>
    {
        public override void Configure(EntityTypeBuilder<UserItemCategory> builder)
        {
            base.Configure(builder);
            builder.HasOne(x => x.User)
                .WithMany(x => x.UserItemCategories)
                .HasForeignKey(x => x.UserId)
                .IsRequired();

            builder.HasOne(x => x.ItemCategory)
                .WithMany(x => x.UserItemCategories)
                .HasForeignKey(x => x.ItemCategoryId)
                .IsRequired();
        }
    }
}