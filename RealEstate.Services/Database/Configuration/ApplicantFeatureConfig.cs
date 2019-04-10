using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Services.Database.Base;
using RealEstate.Services.Database.Tables;

namespace RealEstate.Services.Database.Configuration
{
    public class ApplicantFeatureConfig : BaseEntityConfiguration<ApplicantFeature>
    {
        public override void Configure(EntityTypeBuilder<ApplicantFeature> builder)
        {
            base.Configure(builder);
            builder.HasOne(x => x.Applicant)
                .WithMany(x => x.ApplicantFeatures)
                .HasForeignKey(x => x.ApplicantId)
                .IsRequired();

            builder.HasOne(x => x.Feature)
                .WithMany(x => x.ApplicantFeatures)
                .HasForeignKey(x => x.FeatureId)
                .IsRequired();
        }
    }
}