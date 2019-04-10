using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Services.Database.Base;
using RealEstate.Services.Database.Tables;

namespace RealEstate.Services.Database.Configuration
{
    public class PropertyConfig : BaseEntityConfiguration<Property>
    {
        public override void Configure(EntityTypeBuilder<Property> builder)
        {
            base.Configure(builder);
            builder.HasOne(x => x.District)
                .WithMany(x => x.Properties)
                .HasForeignKey(x => x.DistrictId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

           
        }
    }
}