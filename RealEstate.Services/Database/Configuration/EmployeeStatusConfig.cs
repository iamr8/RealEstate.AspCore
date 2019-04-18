using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Services.Database.Base;
using RealEstate.Services.Database.Tables;

namespace RealEstate.Services.Database.Configuration
{
    public class EmployeeStatusConfig : BaseEntityConfiguration<EmployeeStatus>
    {
        public override void Configure(EntityTypeBuilder<EmployeeStatus> builder)
        {
            base.Configure(builder);
            builder.HasOne(x => x.Employee)
                .WithMany(x => x.EmployeeStatuses)
                .HasForeignKey(x => x.EmployeeId)
                .IsRequired();
        }
    }
}