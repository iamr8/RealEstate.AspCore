using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Services.Database.Base;
using RealEstate.Services.Database.Tables;

namespace RealEstate.Services.Database.Configuration
{
    public class EmployeeDivisionConfig : BaseEntityConfiguration<EmployeeDivision>
    {
        public override void Configure(EntityTypeBuilder<EmployeeDivision> builder)
        {
            base.Configure(builder);
            builder.HasOne(x => x.Employee)
                .WithMany(x => x.EmployeeDivisions)
                .HasForeignKey(x => x.EmployeeId)
                .IsRequired();
        }
    }
}