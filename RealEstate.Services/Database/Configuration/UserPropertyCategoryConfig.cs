using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Services.Database.Base;
using RealEstate.Services.Database.Tables;

namespace RealEstate.Services.Database.Configuration
{
    public class UserPropertyCategoryConfig : BaseEntityConfiguration<UserPropertyCategory>
    {
        public override void Configure(EntityTypeBuilder<UserPropertyCategory> builder)
        {
            base.Configure(builder);
            builder.HasOne(x => x.User)
                .WithMany(x => x.UserPropertyCategories)
                .HasForeignKey(x => x.UserId)
                .IsRequired();

            builder.HasOne(x => x.Category)
                .WithMany(x => x.UserPropertyCategories)
                .HasForeignKey(x => x.CategoryId)
                .IsRequired();
        }
    }
}