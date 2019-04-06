using GeoAPI.Geometries;
using RealEstate.Domain.Tables;
using RealEstate.ViewModels;
using RealEstate.ViewModels.Json;
using System.Collections.Generic;
using System.Linq;

namespace RealEstate.Services.Base
{
    public interface IMapService
    {
        List<ApplicantViewModel> Map(List<Applicant> model);

        UserCategoryJsonViewModel MapJson(UserPropertyCategory model);

        List<FeatureViewModel> Map(List<Feature> model);

        UserCategoryJsonViewModel MapJson(UserItemCategory model);

        List<UserCategoryJsonViewModel> MapJson(List<UserItemCategory> model);

        List<UserCategoryJsonViewModel> MapJson(List<UserPropertyCategory> model);

        List<PermissionViewModel> Map(List<Permission> model);

        PermissionViewModel Map(Permission model);

        List<PropertyViewModel> Map(List<Property> model);

        List<UserViewModel> Map(List<User> model);

        CategoryViewModel Map(UserPropertyCategory model);

        UserViewModel Map(User model);

        SmsTemplateViewModel Map(SmsTemplate model);

        List<SmsViewModel> Map(List<Sms> model);

        SmsViewModel Map(Sms model);

        List<SmsTemplateViewModel> Map(List<SmsTemplate> model);

        List<CategoryViewModel> Map(List<UserPropertyCategory> model);

        PropertyOwnershipViewModel Map(PropertyOwnership model);

        PropertyViewModel Map(Property model);

        List<PropertyOwnershipViewModel> Map(List<PropertyOwnership> model);

        DealPaymentViewModel Map(DealPayment model);

        List<BeneficiaryViewModel> Map(List<Beneficiary> model);

        BeneficiaryViewModel Map(Beneficiary model);

        List<DealPaymentViewModel> Map(List<DealPayment> model);

        DealViewModel Map(Deal model);

        List<CategoryViewModel> Map(List<Category> model);

        List<PaymentViewModel> Map(List<Payment> model);

        PaymentViewModel Map(Payment model);

        List<CategoryViewModel> Map(List<UserItemCategory> model);

        DistrictViewModel Map(District model);

        GeolocationViewModel Map(IPoint model);

        CategoryViewModel Map(UserItemCategory model);

        List<ItemRequestViewModel> Map(List<ItemRequest> model);

        List<ItemViewModel> Map(List<Item> model);

        ItemRequestViewModel Map(ItemRequest model);

        CategoryViewModel Map(Category model);

        ItemViewModel Map(Item model);

        List<FacilityViewModel> Map(List<PropertyFacility> model);

        List<FeatureValueViewModel> Map(List<PropertyFeature> model);

        FeatureValueViewModel Map(ApplicantFeature model);

        List<FeatureValueViewModel> Map(List<ApplicantFeature> model);

        List<FeatureValueViewModel> Map(List<ItemFeature> model);

        FeatureValueViewModel Map(ItemFeature model);

        FeatureViewModel Map(Feature model);

        FeatureValueViewModel Map(PropertyFeature model);

        FacilityViewModel Map(PropertyFacility model);

        List<PictureViewModel> Map(List<Picture> model);

        PictureViewModel Map(Picture model);

        OwnershipViewModel Map(Ownership model);

        ApplicantViewModel Map(Applicant model);

        List<OwnershipViewModel> Map(List<Ownership> model);

        ContactViewModel Map(Contact model);
    }

    public class MapService : IMapService
    {
        private readonly IBaseService _baseService;

        public MapService(
            IBaseService baseService
            )
        {
            _baseService = baseService;
        }

        public List<ApplicantViewModel> Map(List<Applicant> model)
        {
            if (model?.Any() != true)
                return default;

            var result = _baseService.Map(model, Map);
            return result;
        }

        public PictureViewModel Map(Picture model)
        {
            if (model == null)
                return default;

            var result = _baseService.Map(model,
                new PictureViewModel
                {
                    File = model.File,
                    Text = model.Text,
                });
            return result;
        }

        public List<PictureViewModel> Map(List<Picture> model)
        {
            if (model?.Any() != true)
                return default;

            var result = _baseService.Map(model, Map);
            return result;
        }

        public DealViewModel Map(Deal model)
        {
            if (model == null)
                return default;

            var result = _baseService.Map(model,
                new DealViewModel
                {
                    ItemRequest = Map(model.ItemRequest),
                    DealPayments = Map(model.DealPayments.ToList()),
                    Beneficiaries = Map(model.Beneficiaries.ToList()),
                    Pictures = Map(model.Pictures.ToList())
                });
            return result;
        }

        public ApplicantViewModel Map(Applicant model)
        {
            if (model == null)
                return default;

            var result = _baseService.Map(model,
                new ApplicantViewModel
                {
                    Contact = Map(model.Contact),
                    Features = Map(model.ApplicantFeatures.ToList()),
                    Type = model.Type,
                    User = Map(model.User)
                });
            return result;
        }

        public OwnershipViewModel Map(Ownership model)
        {
            if (model == null)
                return default;

            var result = _baseService.Map(model,
                new OwnershipViewModel
                {
                    Contact = Map(model.Contact),
                    Dong = model.Dong,
                    Id = model.Id,
                    PropertyOwnershipId = model.PropertyOwnershipId,
                });

            return result;
        }

        public List<OwnershipViewModel> Map(List<Ownership> model)
        {
            if (model?.Any() != true)
                return default;

            var result = _baseService.Map(model, Map);
            return result;
        }

        public ContactViewModel Map(Contact model)
        {
            if (model == null)
                return default;

            var result = _baseService.Map(model,
                new ContactViewModel
                {
                    Id = model.Id,
                    Description = model.Description,
                    Address = model.Address,
                    Mobile = model.MobileNumber,
                    Name = model.Name,
                    Phone = model.PhoneNumber,
                    Smses = Map(model.Smses.ToList()),
                    Applicants = Map(model.Applicants.ToList()),
                    Ownerships = Map(model.Ownerships.ToList())
                });
            return result;
        }

        public FacilityViewModel Map(PropertyFacility model)
        {
            if (model == null)
                return default;

            var result = _baseService.Map(model,
                new FacilityViewModel
                {
                    Id = model.Id,
                    Name = model.Facility.Name
                });
            return result;
        }

        public List<PropertyViewModel> Map(List<Property> model)
        {
            if (model?.Any() != true)
                return default;

            var result = _baseService.Map(model, Map);
            return result;
        }

        public List<FacilityViewModel> Map(List<PropertyFacility> model)
        {
            if (model?.Any() != true)
                return default;

            var result = _baseService.Map(model, Map);
            return result;
        }

        public FeatureViewModel Map(Feature model)
        {
            if (model == null)
                return default;

            var result = _baseService.Map(model,
                new FeatureViewModel
                {
                    Id = model.Id,
                    Name = model.Name,
                    Type = model.Type,
                    Properties = model.PropertyFeatures.Count
                });
            return result;
        }

        public FeatureValueViewModel Map(ApplicantFeature model)
        {
            if (model == null)
                return default;

            var result = _baseService.Map(model,
                new FeatureValueViewModel
                {
                    Id = model.Id,
                    Feature = Map(model.Feature)
                });
            return result;
        }

        public FeatureValueViewModel Map(ItemFeature model)
        {
            if (model == null)
                return default;

            var result = _baseService.Map(model,
                new FeatureValueViewModel
                {
                    Id = model.Id,
                    Feature = Map(model.Feature)
                });
            return result;
        }

        public FeatureValueViewModel Map(PropertyFeature model)
        {
            if (model == null)
                return default;

            var result = _baseService.Map(model,
                new FeatureValueViewModel
                {
                    Id = model.Id,
                    Feature = Map(model.Feature)
                });
            return result;
        }

        public List<FeatureValueViewModel> Map(List<ApplicantFeature> model)
        {
            if (model?.Any() != true)
                return default;

            var result = _baseService.Map(model, Map);
            return result;
        }

        public List<FeatureValueViewModel> Map(List<ItemFeature> model)
        {
            if (model?.Any() != true)
                return default;

            var result = _baseService.Map(model, Map);
            return result;
        }

        public List<FeatureValueViewModel> Map(List<PropertyFeature> model)
        {
            if (model?.Any() != true)
                return default;

            var result = _baseService.Map(model, Map);
            return result;
        }

        public CategoryViewModel Map(UserItemCategory model)
        {
            if (model == null)
                return null;

            var result = _baseService.Map(model, Map(model.Category));
            return result;
        }

        public ItemRequestViewModel Map(ItemRequest model)
        {
            if (model == null)
                return default;

            var result = _baseService.Map(model,
                new ItemRequestViewModel
                {
                    Applicants = Map(model.Applicants.ToList()),
                    Item = Map(model.Item),
                    IsReject = model.IsReject
                });
            return result;
        }

        public List<ItemRequestViewModel> Map(List<ItemRequest> model)
        {
            if (model?.Any() != true)
                return default;

            var result = _baseService.Map(model, Map);
            return result;
        }

        public ItemViewModel Map(Item model)
        {
            if (model == null)
                return default;

            var result = _baseService.Map(model,
                new ItemViewModel
                {
                    Category = Map(model.Category),
                    Description = model.Description,
                    Features = Map(model.ItemFeatures.ToList()),
                    IsRequested = model.ItemRequests.OrderByDescending(x => x.DateTime).FirstOrDefault()?.IsReject != true,
                    Property = Map(model.Property)
                });
            return result;
        }

        public List<ItemViewModel> Map(List<Item> model)
        {
            if (model?.Any() != true)
                return default;

            var result = _baseService.Map(model, Map);
            return result;
        }

        public List<FeatureViewModel> Map(List<Feature> model)
        {
            if (model?.Any() != true)
                return default;

            var result = _baseService.Map(model, Map);
            return result;
        }

        public List<CategoryViewModel> Map(List<Category> model)
        {
            if (model?.Any() != true)
                return default;

            var result = _baseService.Map(model, Map);
            return result;
        }

        public List<CategoryViewModel> Map(List<UserItemCategory> model)
        {
            if (model?.Any() != true)
                return default;

            var result = _baseService.Map(model, Map);
            return result;
        }

        public GeolocationViewModel Map(IPoint model)
        {
            if (model == null)
                return default;

            var result = _baseService.Map(model,
                new GeolocationViewModel
                {
                    Point = model,
                    Longitude = model.X,
                    Latitude = model.Y
                });
            return result;
        }

        public DistrictViewModel Map(District model)
        {
            if (model == null)
                return default;

            var result = _baseService.Map(model,
                new DistrictViewModel
                {
                    Name = model.Name,
                    Properties = model.Properties.Count
                });

            return result;
        }

        public DealPaymentViewModel Map(DealPayment model)
        {
            if (model == null)
                return default;

            var result = _baseService.Map(model,
                new DealPaymentViewModel
                {
                    Pictures = Map(model.Pictures.ToList()),
                    Detail = new DealPaymentJsonViewModel
                    {
                        Commission = model.CommissionPrice,
                        PayDate = model.PayDate,
                        Text = model.Text,
                        Tip = model.TipPrice,
                    }
                });
            return result;
        }

        public List<BeneficiaryViewModel> Map(List<Beneficiary> model)
        {
            if (model?.Any() != true)
                return default;

            var result = _baseService.Map(model, Map);
            return result;
        }

        public BeneficiaryViewModel Map(Beneficiary model)
        {
            if (model == null)
                return default;

            var result = _baseService.Map(model,
                new BeneficiaryViewModel
                {
                    CommissionPercent = model.CommissionPercent,
                    TipPercent = model.TipPercent,
                    User = Map(model.User)
                });
            return result;
        }

        public List<DealPaymentViewModel> Map(List<DealPayment> model)
        {
            if (model?.Any() != true)
                return default;

            var result = _baseService.Map(model, Map);
            return result;
        }

        public PropertyViewModel Map(Property model)
        {
            if (model == null)
                return default;

            var result = _baseService.Map(model,
                new PropertyViewModel
                {
                    Flat = model.Flat,
                    Floor = model.Floor,
                    Number = model.Number,
                    Alley = model.Alley,
                    Street = model.Street,
                    BuildingName = model.BuildingName,
                    Category = Map(model.Category),
                    Description = model.Description,
                    Deals = model.Items.Sum(x => x.ItemRequests.Count(c => c.Deal != null)),
                    District = Map(model.District),
                    Facilities = Map(model.PropertyFacilities.ToList()),
                    Features = Map(model.PropertyFeatures.ToList()),
                    Ownerships = Map(model.PropertyOwnerships.ToList()),
                    Pictures = Map(model.Pictures.ToList()),
                    Items = Map(model.Items.ToList()),
                    Geolocation = Map(model.Geolocation)
                });
            return result;
        }

        public List<CategoryViewModel> Map(List<UserPropertyCategory> model)
        {
            if (model?.Any() != true)
                return default;

            var result = _baseService.Map(model, Map);
            return result;
        }

        public List<UserCategoryJsonViewModel> MapJson(List<UserItemCategory> model)
        {
            if (model?.Any() != true)
                return default;

            var result = _baseService.Map(model, MapJson);
            return result;
        }

        public UserCategoryJsonViewModel MapJson(UserItemCategory model)
        {
            if (model == null)
                return null;

            var result = _baseService.Map(model, new UserCategoryJsonViewModel
            {
                Id = model.CategoryId,
                Name = model.Category.Name
            });
            return result;
        }

        public List<UserCategoryJsonViewModel> MapJson(List<UserPropertyCategory> model)
        {
            if (model?.Any() != true)
                return default;

            var result = _baseService.Map(model, MapJson);
            return result;
        }

        public UserCategoryJsonViewModel MapJson(UserPropertyCategory model)
        {
            if (model == null)
                return null;

            var result = _baseService.Map(model, new UserCategoryJsonViewModel
            {
                Id = model.CategoryId,
                Name = model.Category.Name
            });
            return result;
        }

        public CategoryViewModel Map(UserPropertyCategory model)
        {
            if (model == null)
                return null;

            var result = _baseService.Map(model, Map(model.Category));
            return result;
        }

        public List<PropertyOwnershipViewModel> Map(List<PropertyOwnership> model)
        {
            if (model?.Any() != true)
                return default;

            var result = _baseService.Map(model, Map);
            return result;
        }

        public PropertyOwnershipViewModel Map(PropertyOwnership model)
        {
            if (model == null)
                return default;

            var result = _baseService.Map(model,
                new PropertyOwnershipViewModel
                {
                    Owners = Map(model.Ownerships.ToList()),
                });

            return result;
        }

        public CategoryViewModel Map(Category model)
        {
            if (model == null)
                return default;

            var result = _baseService.Map(model,
                new CategoryViewModel
                {
                    Id = model.Id,
                    Name = model.Name,
                    Type = model.Type,
                    Properties = Map(model.Properties.ToList()),
                    Items = Map(model.Items.ToList())
                });

            return result;
        }

        public SmsTemplateViewModel Map(SmsTemplate model)
        {
            if (model == null)
                return default;

            var result = _baseService.Map(model,
                new SmsTemplateViewModel
                {
                    Id = model.Id,
                    Text = model.Text,
                    Smses = Map(model.Smses.ToList())
                });

            return result;
        }

        public List<SmsTemplateViewModel> Map(List<SmsTemplate> model)
        {
            if (model?.Any() != true)
                return default;

            var result = _baseService.Map(model, Map);
            return result;
        }

        public SmsViewModel Map(Sms model)
        {
            if (model == null)
                return default;

            var result = _baseService.Map(model,
                new SmsViewModel
                {
                    Id = model.Id,
                    Text = model.Text,
                    User = Map(model.User),
                    SmsTemplate = Map(model.SmsTemplate),
                    Contact = Map(model.Contact),
                    Provider = model.Provider,
                    Sender = model.Sender,
                    Receiver = model.Receiver,
                    ReferenceId = model.ReferenceId,
                    StatusJson = model.StatusJson
                });

            return result;
        }

        public List<SmsViewModel> Map(List<Sms> model)
        {
            if (model?.Any() != true)
                return default;

            var result = _baseService.Map(model, Map);
            return result;
        }

        public PermissionViewModel Map(Permission model)
        {
            if (model == null)
                return default;

            var result = _baseService.Map(model,
                new PermissionViewModel
                {
                    Type = model.Type,
                    Key = model.Key
                });

            return result;
        }

        public PaymentViewModel Map(Payment model)
        {
            if (model == null)
                return default;

            var result = _baseService.Map(model,
                new PaymentViewModel
                {
                    Id = model.Id,
                    Text = model.Text,
                    Type = model.Type,
                    Value = model.Value,
                    Pictures = Map(model.Pictures.ToList())
                });

            return result;
        }

        public List<PermissionViewModel> Map(List<Permission> model)
        {
            if (model?.Any() != true)
                return default;

            var result = _baseService.Map(model, Map);
            return result;
        }

        public List<PaymentViewModel> Map(List<Payment> model)
        {
            if (model?.Any() != true)
                return default;

            var result = _baseService.Map(model, Map);
            return result;
        }

        public List<UserViewModel> Map(List<User> model)
        {
            if (model?.Any() != true)
                return default;

            var result = _baseService.Map(model, Map);
            return result;
        }

        public UserViewModel Map(User model)
        {
            if (model == null) return null;

            var result = _baseService.Map(model,
                new UserViewModel
                {
                    Role = model.Role,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Mobile = model.Mobile,
                    EncryptedPassword = model.Password,
                    Username = model.Username,
                    Address = model.Address,
                    Phone = model.Phone,
                    CreationDateTime = model.DateTime,
                    ItemCategories = Map(model.UserItemCategories.ToList()),
                    PropertyCategories = Map(model.UserPropertyCategories.ToList()),
                    Beneficiaries = Map(model.Beneficiaries.ToList()),
                    Applicants = Map(model.Applicants.ToList()),
                    Payments = Map(model.Payments.ToList()),
                    Smses = Map(model.Smses.ToList()),
                    Permissions = Map(model.Permissions.ToList())
                });
            return result;
        }
    }
}