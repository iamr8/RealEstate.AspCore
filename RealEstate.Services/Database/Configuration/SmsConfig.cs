using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Services.Database.Base;
using RealEstate.Services.Database.Tables;

namespace RealEstate.Services.Database.Configuration
{
    public class SmsConfig : BaseEntityConfiguration<Sms>
    {
        public override void Configure(EntityTypeBuilder<Sms> builder)
        {
            base.Configure(builder);

            builder.HasOne(x => x.Customer)
                .WithMany(x => x.Smses)
                .HasForeignKey(x => x.CustomerId);

            builder.HasOne(x => x.Employee)
                .WithMany(x => x.Smses)
                .HasForeignKey(x => x.EmployeeId);
        }
    }
}