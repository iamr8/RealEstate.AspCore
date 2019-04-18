using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Services.Database.Base;
using RealEstate.Services.Database.Tables;

namespace RealEstate.Services.Database.Configuration
{
    public class ApplicantConfig : BaseEntityConfiguration<Applicant>
    {
        public override void Configure(EntityTypeBuilder<Applicant> builder)
        {
            base.Configure(builder);
            builder.HasOne(x => x.Customer)
                .WithMany(x => x.Applicants)
                .HasForeignKey(x => x.CustomerId)
                .IsRequired();

            builder.HasOne(x => x.User)
                .WithMany(x => x.Applicants)
                .HasForeignKey(x => x.UserId)
                .IsRequired();
        }
    }
}