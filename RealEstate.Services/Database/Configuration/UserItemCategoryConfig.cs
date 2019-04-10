using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Services.Database.Base;
using RealEstate.Services.Database.Tables;

namespace RealEstate.Services.Database.Configuration
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

            builder.HasOne(x => x.Category)
                .WithMany(x => x.UserItemCategories)
                .HasForeignKey(x => x.CategoryId)
                .IsRequired();
        }
    }
}