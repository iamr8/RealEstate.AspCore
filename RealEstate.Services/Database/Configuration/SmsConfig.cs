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

            builder.HasOne(x => x.Contact)
                .WithMany(x => x.Smses)
                .HasForeignKey(x => x.ContactId);

            builder.HasOne(x => x.SmsTemplate)
                .WithMany(x => x.Smses)
                .HasForeignKey(x => x.SmsTemplateId);

            builder.HasOne(x => x.User)
                .WithMany(x => x.Smses)
                .HasForeignKey(x => x.UserId);
        }
    }
}