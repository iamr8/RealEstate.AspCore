using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Base.Database;
using RealEstate.Domain.Tables;

namespace RealEstate.Domain.Configuration
{
    internal class ItemCategoryConfig : BaseEntityConfiguration<ItemCategory>
    {
        public override void Configure(EntityTypeBuilder<ItemCategory> builder)
        {
            base.Configure(builder);
        }
    }
}