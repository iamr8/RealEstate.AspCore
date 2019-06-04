using RealEstate.Resources;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Base.Enums
{
    public enum StatusEnum
    {
        [Display(ResourceType = typeof(SharedResource), Name = "PendingRequest")]
        Pending,

        [Display(ResourceType = typeof(SharedResource), Name = "SuccessfullyDone")]
        Success,

        [Display(ResourceType = typeof(SharedResource), Name = "ModelNotFound")]
        ModelIsNull,

        [Display(ResourceType = typeof(SharedResource), Name = "UnableToSave")]
        UnableToSave,

        [Display(ResourceType = typeof(SharedResource), Name = "CurrentUserIsUnreachable")]
        UserIsNull,

        [Display(ResourceType = typeof(SharedResource), Name = "NullParam")]
        ParamIsNull,

        [Display(ResourceType = typeof(SharedResource), Name = "AlreadyExists")]
        AlreadyExists,

        CustomerIsNull,
        DistrictIsNull,
        PropertyIsNull,
        NoNeedToSave,
        Forbidden,

        [Display(ResourceType = typeof(SharedResource), Name = "PartialSuccess")]
        PartialSuccess,

        [Display(ResourceType = typeof(SharedResource), Name = "RetryAfterReview")]
        RetryAfterReview,

        AlreadyDeleted,

        [Display(ResourceType = typeof(SharedResource), Name = "SuccessfullyDone")]
        SignedIn,

        [Display(ResourceType = typeof(SharedResource), Name = "Deactivated")]
        Deactivated,

        [Display(ResourceType = typeof(SharedResource), Name = "Failed")]
        Failed,

        [Display(ResourceType = typeof(SharedResource), Name = "UserNotFound")]
        UserNotFound,

        [Display(ResourceType = typeof(SharedResource), Name = "WrongPassword")]
        WrongPassword,

        [Display(ResourceType = typeof(SharedResource), Name = "AlreadySignedIn")]
        AlreadySignedIn,

        DealStatusIsNull,

        [Display(ResourceType = typeof(SharedResource), Name = "ForbiddenAndUnableToUpdateOrShow")]
        ForbiddenAndUnableToUpdateOrShow,

        [Display(ResourceType = typeof(SharedResource), Name = "SameAsOwner")]
        SameAsOwner,

        ItemIsNull,
        NotificationIsNull,
        ApplicantsEmpty,
        ApplicantIsNull,
        FileIsNull,
        PropertyOwnershipIsNull,
        OwnershipIsNull,
        PropertyOwnershipNotUpdated,
        DealRequestIsNull,

        RecipientIsNull,
        TemplateIsNull,
        TokensCountMismatch,
        UnexpectedError,
        TokenIsNull,
        IdIsNull,
        EmployeeIsNull,
        DivisionIdIsNull,
        NameIsNull,
        EntityIsNull,
        DealIsNull,
        ManagemenetPercentIsNull,
        LeaveIsNull,
        Ready,
        CredentialError,
        PriceIsNull,
        PaymentsAreEmpty,
        CheckoutIsNull,
        RemainIsNull,
        PaymentIsNull,

        [Display(ResourceType = typeof(SharedResource), Name = "PropertyIsAlreadyExists")]
        PropertyIsAlreadyExists
    }
}