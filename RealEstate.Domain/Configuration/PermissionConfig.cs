using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.Base;
using RealEstate.Domain.Tables;

namespace RealEstate.Domain.Configuration
{
    public class PermissionConfig : BaseEntityConfiguration<Permission>
    {
        public override void Configure(EntityTypeBuilder<Permission> builder)
        {
            base.Configure(builder);

            builder.HasOne(x => x.User)
                .WithMany(x => x.Permissions)
                .HasForeignKey(x => x.UserId)
                .IsRequired();
        }
    }
}