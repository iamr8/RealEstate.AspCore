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

            builder.HasMany(x => x.DealRequests)
                .WithOne(x => x.Sms)
                .HasForeignKey(x => x.SmsId);

            builder.HasMany(x => x.Payments)
                .WithOne(x => x.Sms)
                .HasForeignKey(x => x.SmsId);
        }
    }
}