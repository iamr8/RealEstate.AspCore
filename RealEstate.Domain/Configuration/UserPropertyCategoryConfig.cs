using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Base.Database;
using RealEstate.Domain.Tables;

namespace RealEstate.Domain.Configuration
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

            builder.HasOne(x => x.PropertyCategory)
                .WithMany(x => x.UserPropertyCategories)
                .HasForeignKey(x => x.PropertyCategoryId)
                .IsRequired();
        }
    }
}