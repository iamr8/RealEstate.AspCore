using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Base.Database;
using RealEstate.Domain.Tables;

namespace RealEstate.Domain.Configuration
{
    public class NotificationSeenerConfig : BaseEntityConfiguration<NotificationSeener>
    {
        public override void Configure(EntityTypeBuilder<NotificationSeener> builder)
        {
            base.Configure(builder);
            builder.HasOne(x => x.Log)
                .WithMany(x => x.NotificationSeeners)
                .HasForeignKey(x => x.LogId)
                .IsRequired();

            builder.HasOne(x => x.User)
                .WithMany(x => x.NotificationSeeners)
                .HasForeignKey(x => x.UserId)
                .IsRequired();
        }
    }
}