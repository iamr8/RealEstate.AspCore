using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Services.Database.Base;
using RealEstate.Services.Database.Tables;

namespace RealEstate.Services.Database.Configuration
{
    public class ManagementPercentConfig : BaseEntityConfiguration<ManagementPercent>
    {
        public override void Configure(EntityTypeBuilder<ManagementPercent> builder)
        {
            base.Configure(builder);
            builder.HasOne(x => x.Employee)
                .WithMany(x => x.ManagementPercents)
                .HasForeignKey(x => x.EmployeeId)
                .IsRequired();
        }
    }
}