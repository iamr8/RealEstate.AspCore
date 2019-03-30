using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Base.Database;
using RealEstate.Domain.Tables;

namespace RealEstate.Domain.Configuration
{
    public class LogConfig : BaseEntityConfiguration<Log>
    {
        public override void Configure(EntityTypeBuilder<Log> builder)
        {
            base.Configure(builder);

            builder.HasMany(x => x.NotificationRecipients)
                .WithOne(x => x.Log)
                .HasForeignKey(x => x.LogId)
                .IsRequired();
        }
    }
}