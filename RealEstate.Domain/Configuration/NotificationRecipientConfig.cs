using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Base.Database;
using RealEstate.Domain.Tables;

namespace RealEstate.Domain.Configuration
{
    public class NotificationRecipientConfig : BaseEntityConfiguration<NotificationRecipient>
    {
        public override void Configure(EntityTypeBuilder<NotificationRecipient> builder)
        {
            base.Configure(builder);
            builder.HasOne(x => x.User)
                .WithMany(x => x.NotificationRecipients)
                .HasForeignKey(x => x.UserId)
                .IsRequired();
        }
    }
}