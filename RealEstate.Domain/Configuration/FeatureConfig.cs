using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Base.Database;
using RealEstate.Domain.Tables;

namespace RealEstate.Domain.Configuration
{
    public class FeatureConfig : BaseEntityConfiguration<Feature>
    {
        public override void Configure(EntityTypeBuilder<Feature> builder)
        {
            base.Configure(builder);
        }
    }
}