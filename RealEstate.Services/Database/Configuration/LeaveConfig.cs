using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Services.Database.Base;
using RealEstate.Services.Database.Tables;

namespace RealEstate.Services.Database.Configuration
{
    public class LeaveConfig : BaseEntityConfiguration<Leave>
    {
        public override void Configure(EntityTypeBuilder<Leave> builder)
        {
            base.Configure(builder);
            builder.HasOne(x => x.Employee)
                .WithMany(x => x.Leaves)
                .HasForeignKey(x => x.EmployeeId)
                .IsRequired();
        }
    }
}