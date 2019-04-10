using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Services.Database.Base;
using RealEstate.Services.Database.Tables;

namespace RealEstate.Services.Database.Configuration
{
    public class SmsTemplateConfig : BaseEntityConfiguration<SmsTemplate>
    {
        public override void Configure(EntityTypeBuilder<SmsTemplate> builder)
        {
            base.Configure(builder);
        }
    }
}