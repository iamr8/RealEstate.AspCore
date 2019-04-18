using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Services.Database.Base;
using RealEstate.Services.Database.Tables;

namespace RealEstate.Services.Database.Configuration
{
    public class FixedSalariConfig : BaseEntityConfiguration<FixedSalary>
    {
        public override void Configure(EntityTypeBuilder<FixedSalary> builder)
        {
            base.Configure(builder);
            builder.HasOne(x => x.Employee)
                .WithMany(x => x.FixedSalaries)
                .HasForeignKey(x => x.EmployeeId)
                .IsRequired();
        }
    }
}