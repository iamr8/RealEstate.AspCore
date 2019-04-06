using RealEstate.Base.Enums;
using RealEstate.Domain.Base;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Domain.Tables
{
    public class Log : BaseEntity
    {
        public LogTypeEnum Type { get; set; }

        [Required]
        public string CreatorId { get; set; }

        public string ApplicantId { get; set; }
        public virtual Applicant Applicant { get; set; }

        public string ApplicantFeatureId { get; set; }
        public virtual ApplicantFeature ApplicantFeature { get; set; }
        public string BeneficiaryId { get; set; }
        public virtual Beneficiary Beneficiary { get; set; }
        public string ContactId { get; set; }
        public virtual Contact Contact { get; set; }
        public string DealId { get; set; }
        public virtual Deal Deal { get; set; }
        public string DealPaymentId { get; set; }
        public virtual DealPayment DealPayment { get; set; }
        public string DistrictId { get; set; }
        public virtual District District { get; set; }
        public string FacilityId { get; set; }
        public virtual Facility Facility { get; set; }
        public string FeatureId { get; set; }
        public virtual Feature Feature { get; set; }
        public string ItemId { get; set; }
        public string FixedSalaryId { get; set; }
        public virtual FixedSalary FixedSalary { get; set; }
        public virtual Item Item { get; set; }
        public string CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public string ItemFeatureId { get; set; }
        public virtual ItemFeature ItemFeature { get; set; }
        public string OwnershipId { get; set; }
        public virtual Ownership Ownership { get; set; }
        public string PictureId { get; set; }
        public virtual Picture Picture { get; set; }
        public string PropertyId { get; set; }
        public virtual Property Property { get; set; }
        public string PropertyFacilityId { get; set; }
        public virtual PropertyFacility PropertyFacility { get; set; }
        public string PropertyFeatureId { get; set; }
        public virtual PropertyFeature PropertyFeature { get; set; }
        public string PropertyOwnershipId { get; set; }
        public virtual PropertyOwnership PropertyOwnership { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public string UserItemCategoryId { get; set; }
        public virtual UserItemCategory UserItemCategory { get; set; }
        public string UserPropertyCategoryId { get; set; }
        public virtual UserPropertyCategory UserPropertyCategory { get; set; }
        public string PaymentId { get; set; }
        public virtual Payment Payment { get; set; }
        public string ItemRequestId { get; set; }
        public virtual ItemRequest ItemRequest { get; set; }
        public string SmsTemplateId { get; set; }
        public virtual SmsTemplate SmsTemplate { get; set; }
    }
}