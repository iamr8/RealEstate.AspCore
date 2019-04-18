using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Services.Database.Base;
using RealEstate.Services.Database.Tables;

namespace RealEstate.Services.Database.Configuration
{
    public class PresenceConfig : BaseEntityConfiguration<Presence>
    {
        public override void Configure(EntityTypeBuilder<Presence> builder)
        {
            base.Configure(builder);
            builder.HasOne(x => x.Employee)
                .WithMany(x => x.Presences)
                .HasForeignKey(x => x.EmployeeId)
                .IsRequired();
        }
    }
}