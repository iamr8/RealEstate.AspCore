using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.Base;
using RealEstate.Domain.Tables;

namespace RealEstate.Domain.Configuration
{
    public class SmsTemplateConfig : BaseEntityConfiguration<SmsTemplate>
    {
        public override void Configure(EntityTypeBuilder<SmsTemplate> builder)
        {
            base.Configure(builder);
        }
    }
}