using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.Base;
using RealEstate.Domain.Tables;

namespace RealEstate.Domain.Configuration
{
    public class LogConfig : BaseEntityConfiguration<Log>
    {
        public override void Configure(EntityTypeBuilder<Log> builder)
        {
            base.Configure(builder);

            builder.HasOne(x => x.Picture)
                .WithMany(x => x.Logs)
                .HasForeignKey(x => x.PictureId);

            builder.HasOne(x => x.Applicant)
                .WithMany(x => x.Logs)
                .HasForeignKey(x => x.ApplicantId);

            builder.HasOne(x => x.ApplicantFeature)
                .WithMany(x => x.Logs)
                .HasForeignKey(x => x.ApplicantFeatureId);

            builder.HasOne(x => x.Beneficiary)
                .WithMany(x => x.Logs)
                .HasForeignKey(x => x.BeneficiaryId);

            builder.HasOne(x => x.Contact)
                .WithMany(x => x.Logs)
                .HasForeignKey(x => x.ContactId);

            builder.HasOne(x => x.Deal)
                .WithMany(x => x.Logs)
                .HasForeignKey(x => x.DealId);

            builder.HasOne(x => x.DealPayment)
                .WithMany(x => x.Logs)
                .HasForeignKey(x => x.DealPaymentId);

            builder.HasOne(x => x.District)
                .WithMany(x => x.Logs)
                .HasForeignKey(x => x.DistrictId);

            builder.HasOne(x => x.Facility)
                .WithMany(x => x.Logs)
                .HasForeignKey(x => x.FacilityId);

            builder.HasOne(x => x.Feature)
                .WithMany(x => x.Logs)
                .HasForeignKey(x => x.FeatureId);

            builder.HasOne(x => x.FixedSalary)
                .WithMany(x => x.Logs)
                .HasForeignKey(x => x.FixedSalaryId);

            builder.HasOne(x => x.Item)
                .WithMany(x => x.Logs)
                .HasForeignKey(x => x.ItemId);

            builder.HasOne(x => x.Category)
                .WithMany(x => x.Logs)
                .HasForeignKey(x => x.CategoryId);

            builder.HasOne(x => x.ItemFeature)
                .WithMany(x => x.Logs)
                .HasForeignKey(x => x.ItemFeatureId);

            builder.HasOne(x => x.Ownership)
                .WithMany(x => x.Logs)
                .HasForeignKey(x => x.OwnershipId);

            builder.HasOne(x => x.PropertyOwnership)
                .WithMany(x => x.Logs)
                .HasForeignKey(x => x.PropertyOwnershipId);

            builder.HasOne(x => x.PropertyFeature)
                .WithMany(x => x.Logs)
                .HasForeignKey(x => x.PropertyFeatureId);

            builder.HasOne(x => x.Property)
                .WithMany(x => x.Logs)
                .HasForeignKey(x => x.PropertyId);

            builder.HasOne(x => x.PropertyFacility)
                .WithMany(x => x.Logs)
                .HasForeignKey(x => x.PropertyFacilityId);

            builder.HasOne(x => x.User)
                .WithMany(x => x.Logs)
                .HasForeignKey(x => x.UserId);

            builder.HasOne(x => x.UserPropertyCategory)
                .WithMany(x => x.Logs)
                .HasForeignKey(x => x.UserPropertyCategoryId);

            builder.HasOne(x => x.UserItemCategory)
                .WithMany(x => x.Logs)
                .HasForeignKey(x => x.UserItemCategoryId);

            builder.HasOne(x => x.Payment)
                .WithMany(x => x.Logs)
                .HasForeignKey(x => x.PaymentId);

            builder.HasOne(x => x.ItemRequest)
                .WithMany(x => x.Logs)
                .HasForeignKey(x => x.ItemRequestId);

            builder.HasOne(x => x.SmsTemplate)
                .WithMany(x => x.Logs)
                .HasForeignKey(x => x.SmsTemplateId);
        }
    }
}