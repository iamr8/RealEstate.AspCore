using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.Base;
using RealEstate.Domain.Tables;

namespace RealEstate.Domain.Configuration
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