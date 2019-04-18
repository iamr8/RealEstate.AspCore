using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Services.Database.Base;
using RealEstate.Services.Database.Tables;

namespace RealEstate.Services.Database.Configuration
{
    public class EmployeeConfig : BaseEntityConfiguration<Employee>
    {
        public override void Configure(EntityTypeBuilder<Employee> builder)
        {
            base.Configure(builder);
            builder.HasMany(x => x.Users)
                .WithOne(x => x.Employee)
                .HasForeignKey(x => x.EmployeeId)
                .IsRequired();

            builder.HasIndex(x => x.Mobile)
                .IsUnique();
        }
    }
}