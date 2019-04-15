using GeoAPI.Geometries;
using RealEstate.Base;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace RealEstate.Services.Base
{
    public interface IMapService
    {
        OwnershipViewModel Map(Ownership model, bool includeDeleted);

        CategoryViewModel Map(Category model, bool includeDeleted);
        UserViewModel Map(User model, bool includeDeleted);

        FeatureViewModel Map(Feature model, bool includeDeleted);

        List<PermissionViewModel> Map(ICollection<Permission> model, bool includeDeleted);

        PaymentViewModel Map(Payment model, bool includeDeleted);

        List<PaymentViewModel> Map(ICollection<Payment> model, bool includeDeleted);

        ReminderViewModel Map(Reminder model, bool includeDeleted);

        List<ReminderViewModel> Map(ICollection<Reminder> model, bool includeDeleted);

        UserPropertyCategoryViewModel Map(UserPropertyCategory model, bool includeDeleted);

        SmsViewModel Map(Sms model, bool includeDeleted);

        List<SmsViewModel> Map(ICollection<Sms> model, bool includeDeleted);

        PermissionViewModel Map(Permission model, bool includeDeleted);

        List<FixedSalaryViewModel> Map(ICollection<FixedSalary> model, bool includeDeleted);

        FixedSalaryViewModel Map(FixedSalary model, bool includeDeleted);

        List<UserPropertyCategoryViewModel> Map(ICollection<UserPropertyCategory> model, bool includeDeleted);

        List<UserItemCategoryViewModel> Map(ICollection<UserItemCategory> model, bool includeDeleted);

        UserItemCategoryViewModel Map(UserItemCategory model, bool includeDeleted);

        GeolocationViewModel Map(IPoint model);

        List<PictureViewModel> Map(ICollection<Picture> model, bool includeDeleted);

        PictureViewModel Map(Picture model, bool includeDeleted);

        List<ItemViewModel> Map(ICollection<Item> model, bool includeDeleted);

        List<BeneficiaryViewModel> Map(ICollection<Beneficiary> model, bool includeDeleted);

        List<DealPaymentViewModel> Map(ICollection<DealPayment> model, bool includeDeleted);

        DealPaymentViewModel Map(DealPayment model, bool includeDeleted);

        BeneficiaryViewModel Map(Beneficiary model, bool includeDeleted);

        List<PropertyOwnershipViewModel> Map(ICollection<PropertyOwnership> model, bool includeDeleted);

        List<PropertyFacilityViewModel> Map(ICollection<PropertyFacility> model, bool includeDeleted);

        List<ApplicantViewModel> Map(ICollection<Applicant> model, bool includeDeleted);

        List<FeatureViewModel> Map(ICollection<Feature> model, bool includeDeleted);

        List<FacilityViewModel> Map(ICollection<Facility> model, bool includeDeleted);

        List<ApplicantFeatureViewModel> Map(ICollection<ApplicantFeature> model, bool includeDeleted);

        ApplicantFeatureViewModel Map(ApplicantFeature model, bool includeDeleted);

        DealViewModel Map(Deal model, bool includeDeleted);

        ApplicantViewModel Map(Applicant model, bool includeDeleted);

        List<DistrictViewModel> Map(ICollection<District> model, bool includeDeleted);

        List<ItemFeatureViewModel> Map(ICollection<ItemFeature> model, bool includeDeleted);

        List<OwnershipViewModel> Map(ICollection<Ownership> model, bool includeDeleted);

        List<PropertyFeatureViewModel> Map(ICollection<PropertyFeature> model, bool includeDeleted);

        DistrictViewModel Map(District model, bool includeDeleted);

        PropertyFacilityViewModel Map(PropertyFacility model, bool includeDeleted);

        ItemFeatureViewModel Map(ItemFeature model, bool includeDeleted);

        PropertyOwnershipViewModel Map(PropertyOwnership model, bool includeDeleted);

        PropertyViewModel Map(Property item, bool includeDeleted);

        ItemViewModel Map(Item model, bool includeDeleted);

        PropertyFeatureViewModel Map(PropertyFeature model, bool includeDeleted);

        FacilityViewModel Map(Facility model, bool includeDeleted);

        ContactViewModel Map(Contact model, bool includeDeleted);
    }

    public class MapService : IMapService
    {
        public ContactViewModel Map(Contact model, bool includeDeleted)
        {
            return new ContactViewModel(model, includeDeleted).Instance?
                .R8Include(contact =>
                {
                    contact.Applicants = contact.Entity?.Applicants.Select(propEntity => new ApplicantViewModel(propEntity, false).Instance).R8ToList();
                    contact.Ownerships = contact.Entity?.Ownerships.Select(propEntity => new OwnershipViewModel(propEntity, false).Instance).R8ToList();
                });
        }

        public FacilityViewModel Map(Facility model, bool includeDeleted)
        {
            return new FacilityViewModel(model, includeDeleted).Instance;
        }

        public PropertyFacilityViewModel Map(PropertyFacility model, bool includeDeleted)
        {
            return new PropertyFacilityViewModel(model, includeDeleted).Instance?
                .R8Include(propertyFacility =>
                {
                    propertyFacility.Facility = Map(propertyFacility.Entity?.Facility, false);
                }).ShowBasedOn(b => b.Facility);
        }

        public ItemViewModel Map(Item model, bool includeDeleted)
        {
            return new ItemViewModel(model, includeDeleted).Instance?
                .R8Include(item =>
                {
                    item.Category = Map(item.Entity?.Category, false);
                    item.Features = Map(item.Entity?.ItemFeatures, false);
                    item.Property = Map(item.Entity?.Property, false);
                }).ShowBasedOn(x => x.Property);
        }

        public List<ItemFeatureViewModel> Map(ICollection<ItemFeature> model, bool includeDeleted)
        {
            return model.Select(propEntity => Map(propEntity, includeDeleted)).R8ToList();
        }

        public UserViewModel Map(User model, bool includeDeleted)
        {
            return new UserViewModel(model, includeDeleted).Instance?
                .R8Include(user =>
                {
                    user.ItemCategories = Map(user.Entity?.UserItemCategories, false);
                    user.PropertyCategories = Map(user.Entity?.UserPropertyCategories, false);
                    user.FixedSalaries = Map(user.Entity?.FixedSalaries, false);
                    user.Applicants = Map(user.Entity?.Applicants, false);
                    user.Beneficiaries = Map(user.Entity?.Beneficiaries, false);
                    user.Smses = Map(user.Entity?.Smses, false);
                    user.Permissions = Map(user.Entity?.Permissions, false);
                    user.Reminders = Map(user.Entity?.Reminders, false);
                    user.Payments = Map(user.Entity?.Payments, false);
                });
        }

        public List<PaymentViewModel> Map(ICollection<Payment> model, bool includeDeleted)
        {
            return model.Select(item => Map(item, includeDeleted)).R8ToList();
        }

        public PaymentViewModel Map(Payment model, bool includeDeleted)
        {
            return new PaymentViewModel(model, includeDeleted).Instance;
        }

        public List<ReminderViewModel> Map(ICollection<Reminder> model, bool includeDeleted)
        {
            return model.Select(item => Map(item, includeDeleted)).R8ToList();
        }

        public ReminderViewModel Map(Reminder model, bool includeDeleted)
        {
            return new ReminderViewModel(model, includeDeleted).Instance;
        }

        public List<PermissionViewModel> Map(ICollection<Permission> model, bool includeDeleted)
        {
            return model.Select(item => Map(item, includeDeleted)).R8ToList();
        }

        public PermissionViewModel Map(Permission model, bool includeDeleted)
        {
            return new PermissionViewModel(model, includeDeleted).Instance;
        }

        public List<SmsViewModel> Map(ICollection<Sms> model, bool includeDeleted)
        {
            return model.Select(item => Map(item, includeDeleted)).R8ToList();
        }

        public SmsViewModel Map(Sms model, bool includeDeleted)
        {
            return new SmsViewModel(model, includeDeleted).Instance;
        }

        public List<UserPropertyCategoryViewModel> Map(ICollection<UserPropertyCategory> model, bool includeDeleted)
        {
            return model.Select(item => Map(item, includeDeleted)).R8ToList();
        }

        public List<FixedSalaryViewModel> Map(ICollection<FixedSalary> model, bool includeDeleted)
        {
            return model.Select(item => Map(item, includeDeleted)).R8ToList();
        }

        public FixedSalaryViewModel Map(FixedSalary model, bool includeDeleted)
        {
            return new FixedSalaryViewModel(model, includeDeleted).Instance;
        }

        public UserPropertyCategoryViewModel Map(UserPropertyCategory model, bool includeDeleted)
        {
            return new UserPropertyCategoryViewModel(model, includeDeleted).Instance?
                .R8Include(v =>
                {
                    v.Category = Map(v.Entity?.Category, false);
                });
        }

        public List<UserItemCategoryViewModel> Map(ICollection<UserItemCategory> model, bool includeDeleted)
        {
            return model.Select(item => Map(item, includeDeleted)).R8ToList();
        }

        public UserItemCategoryViewModel Map(UserItemCategory model, bool includeDeleted)
        {
            return new UserItemCategoryViewModel(model, includeDeleted).Instance?
                .R8Include(v =>
                {
                    v.Category = Map(v.Entity?.Category, false);
                });
        }

        public ItemFeatureViewModel Map(ItemFeature model, bool includeDeleted)
        {
            return new ItemFeatureViewModel(model, includeDeleted).Instance?
                .R8Include(itemFeature =>
                {
                    itemFeature.Feature = Map(itemFeature.Entity?.Feature, false);
                }).ShowBasedOn(b => b.Feature);
        }

        public PropertyFeatureViewModel Map(PropertyFeature model, bool includeDeleted)
        {
            return new PropertyFeatureViewModel(model, includeDeleted).Instance?
                .R8Include(propertyFeature =>
                {
                    propertyFeature.Feature = Map(propertyFeature.Entity?.Feature, false);
                }).ShowBasedOn(b => b.Feature);
        }

        public PropertyOwnershipViewModel Map(PropertyOwnership model, bool includeDeleted)
        {
            return new PropertyOwnershipViewModel(model, includeDeleted).Instance?
                .R8Include(propertyOwnership =>
                {
                    propertyOwnership.Ownerships = Map(model.Ownerships, false);
                }).ShowBasedOn(b => b.Ownerships);
        }

        public DistrictViewModel Map(District model, bool includeDeleted)
        {
            return new DistrictViewModel(model, includeDeleted).Instance;
        }

        public List<OwnershipViewModel> Map(ICollection<Ownership> model, bool includeDeleted)
        {
            return model.Select(item => Map(item, includeDeleted)).R8ToList();
        }

        public PropertyViewModel Map(Property item, bool includeDeleted)
        {
            return new PropertyViewModel(item, includeDeleted).Instance?
                .R8Include(property =>
                {
                    property.District = Map(property.Entity?.District, false);
                    property.Category = Map(property.Entity?.Category, false);
                    property.Facilities = Map(property.Entity?.PropertyFacilities, false);
                    property.Features = Map(property.Entity?.PropertyFeatures, false);
                    property.PropertyOwnerships = Map(property.Entity?.PropertyOwnerships, false);
                    property.Geolocation = Map(property.Entity?.Geolocation);
                    property.Items = Map(property.Entity?.Items, false);
                    property.Pictures = Map(property.Entity?.Pictures, false);
                });
        }

        public PictureViewModel Map(Picture model, bool includeDeleted)
        {
            return new PictureViewModel(model, includeDeleted);
        }

        public GeolocationViewModel Map(IPoint model)
        {
            if (model == null)
                return default;

            return new GeolocationViewModel
            {
                Latitude = model.Y,
                Longitude = model.X,
                Point = model
            };
        }

        public CategoryViewModel Map(Category model, bool includeDeleted)
        {
            return new CategoryViewModel(model, includeDeleted).Instance;
        }

        public List<PropertyFacilityViewModel> Map(ICollection<PropertyFacility> model, bool includeDeleted)
        {
            return model.Select(propertyFacility => Map(propertyFacility, includeDeleted)).R8ToList();
        }

        public List<PictureViewModel> Map(ICollection<Picture> model, bool includeDeleted)
        {
            return model.Select(propertyFacility => Map(propertyFacility, includeDeleted)).R8ToList();
        }

        public DealViewModel Map(Deal model, bool includeDeleted)
        {
            return new DealViewModel(model, includeDeleted).Instance?
                .R8Include(c =>
                {
                    c.Applicants = Map(c.Entity?.Applicants, false);
                    c.Item = Map(c.Entity?.Item, false);
                    c.Beneficiaries = Map(c.Entity?.Beneficiaries, false);
                    c.DealPayments = Map(c.Entity?.DealPayments, false);
                    c.Pictures = Map(c.Entity?.Pictures, false);
                }).Instance;
        }

        public DealPaymentViewModel Map(DealPayment model, bool includeDeleted)
        {
            return new DealPaymentViewModel(model, includeDeleted);
        }

        public List<DealPaymentViewModel> Map(ICollection<DealPayment> model, bool includeDeleted)
        {
            return model.Select(c => Map(c, includeDeleted)).R8ToList();
        }

        public List<BeneficiaryViewModel> Map(ICollection<Beneficiary> model, bool includeDeleted)
        {
            return model.Select(c => Map(c, includeDeleted)).R8ToList();
        }

        public BeneficiaryViewModel Map(Beneficiary model, bool includeDeleted)
        {
            return new BeneficiaryViewModel(model, includeDeleted);
        }

        public ApplicantViewModel Map(Applicant model, bool includeDeleted)
        {
            return new ApplicantViewModel(model, false).Instance?.R8Include(x =>
            {
                x.ApplicantFeatures = Map(x.Entity?.ApplicantFeatures, false);
                x.Contact = Map(x.Entity?.Contact, false);
            }).Instance;
        }

        public List<ApplicantFeatureViewModel> Map(ICollection<ApplicantFeature> model, bool includeDeleted)
        {
            return model.Select(c => Map(c, includeDeleted)).R8ToList();
        }

        public ApplicantFeatureViewModel Map(ApplicantFeature model, bool includeDeleted)
        {
            return new ApplicantFeatureViewModel(model, includeDeleted).Instance?
                .R8Include(v =>
                {
                    v.Feature = Map(v.Entity?.Feature, false);
                }).Instance;
        }

        public List<ApplicantViewModel> Map(ICollection<Applicant> model, bool includeDeleted)
        {
            return model.Select(x => Map(x, includeDeleted)).R8ToList();
        }

        public List<DistrictViewModel> Map(ICollection<District> model, bool includeDeleted)
        {
            return model.Select(propertyFacility => Map(propertyFacility, includeDeleted)).R8ToList();
        }

        public FeatureViewModel Map(Feature model, bool includeDeleted)
        {
            return new FeatureViewModel(model, includeDeleted).Instance;
        }

        public List<PropertyFeatureViewModel> Map(ICollection<PropertyFeature> model, bool includeDeleted)
        {
            return model.Select(propertyFeature => Map(propertyFeature, includeDeleted)).R8ToList();
        }

        public List<FacilityViewModel> Map(ICollection<Facility> model, bool includeDeleted)
        {
            return model.Select(propertyOwnership => Map(propertyOwnership, includeDeleted)).R8ToList();
        }

        public List<FeatureViewModel> Map(ICollection<Feature> model, bool includeDeleted)
        {
            return model.Select(propertyOwnership => Map(propertyOwnership, includeDeleted)).R8ToList();
        }

        public List<ItemViewModel> Map(ICollection<Item> model, bool includeDeleted)
        {
            return model.Select(propertyOwnership => Map(propertyOwnership, includeDeleted)).R8ToList();
        }

        public List<PropertyOwnershipViewModel> Map(ICollection<PropertyOwnership> model, bool includeDeleted)
        {
            return model.Select(propertyOwnership => Map(propertyOwnership, includeDeleted)).R8ToList();
        }

        public OwnershipViewModel Map(Ownership model, bool includeDeleted)
        {
            return new OwnershipViewModel(model, includeDeleted).Instance?
                .R8Include(ownership =>
                {
                    ownership.Contact = Map(ownership.Entity.Contact, false);
                }).ShowBasedOn(x => x.Contact);
        }
    }
}