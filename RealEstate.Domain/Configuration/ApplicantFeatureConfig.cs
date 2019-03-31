using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.Base;
using RealEstate.Domain.Tables;

namespace RealEstate.Domain.Configuration
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