using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Services.Database.Base;
using RealEstate.Services.Database.Tables;

namespace RealEstate.Services.Database.Configuration
{
    public class CheckConfig : BaseEntityConfiguration<Check>
    {
        public override void Configure(EntityTypeBuilder<Check> builder)
        {
            base.Configure(builder);
            builder.HasOne(x => x.Deal)
                .WithMany(x => x.Checks)
                .HasForeignKey(x => x.DealId);

            builder.HasOne(x => x.User)
                .WithMany(x => x.Checks)
                .HasForeignKey(x => x.UserId);

            builder.HasMany(x => x.Pictures)
                .WithOne(x => x.Check)
                .HasForeignKey(x => x.CheckId);
        }
    }
}