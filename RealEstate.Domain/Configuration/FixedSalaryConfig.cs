using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.Base;
using RealEstate.Domain.Tables;

namespace RealEstate.Domain.Configuration
{
    public class FixedSalariConfig : BaseEntityConfiguration<FixedSalary>
    {
        public override void Configure(EntityTypeBuilder<FixedSalary> builder)
        {
            base.Configure(builder);
            builder.HasOne(x => x.User)
                .WithMany(x => x.FixedSalaries)
                .HasForeignKey(x => x.UserId)
                .IsRequired();
        }
    }
}