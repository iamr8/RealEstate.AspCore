using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Base.Database;
using RealEstate.Domain.Tables;

namespace RealEstate.Domain.Configuration
{
    public class ApplicantConfig : BaseEntityConfiguration<Applicant>
    {
        public override void Configure(EntityTypeBuilder<Applicant> builder)
        {
            base.Configure(builder);
            builder.HasOne(x => x.Contact)
                .WithMany(x => x.Applicants)
                .HasForeignKey(x => x.ContactId)
                .IsRequired();

            builder.HasOne(x => x.User)
                .WithMany(x => x.Applicants)
                .HasForeignKey(x => x.UserId)
                .IsRequired();
        }
    }
}