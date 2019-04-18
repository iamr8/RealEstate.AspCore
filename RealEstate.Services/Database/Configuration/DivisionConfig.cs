using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Services.Database.Base;
using RealEstate.Services.Database.Tables;

namespace RealEstate.Services.Database.Configuration
{
    public class DivisionConfig : BaseEntityConfiguration<Division>
    {
        public override void Configure(EntityTypeBuilder<Division> builder)
        {
            base.Configure(builder);
            builder.HasMany(x => x.EmployeeDivisions)
                .WithOne(x => x.Division)
                .HasForeignKey(x => x.DivisionId)
                .IsRequired();
        }
    }
}