using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Base.Database;
using RealEstate.Domain.Tables;

namespace RealEstate.Domain.Configuration
{
    public class DistrictConfig : BaseEntityConfiguration<District>
    {
        public override void Configure(EntityTypeBuilder<District> builder)
        {
            base.Configure(builder);
        }
    }
}