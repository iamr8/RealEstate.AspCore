using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Base.Database;
using RealEstate.Domain.Tables;

namespace RealEstate.Domain.Configuration
{
    public class ContactConfig : BaseEntityConfiguration<Contact>
    {
        public override void Configure(EntityTypeBuilder<Contact> builder)
        {
            base.Configure(builder);
        }
    }
}