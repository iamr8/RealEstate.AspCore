using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Services.Database.Base;
using RealEstate.Services.Database.Tables;

namespace RealEstate.Services.Database.Configuration
{
    public class InsuranceConfig : BaseEntityConfiguration<Insurance>
    {
        public override void Configure(EntityTypeBuilder<Insurance> builder)
        {
            base.Configure(builder);
            builder.HasOne(x => x.Employee)
                .WithMany(x => x.Insurances)
                .HasForeignKey(x => x.EmployeeId)
                .IsRequired();
        }
    }
}