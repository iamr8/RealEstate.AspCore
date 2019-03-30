using GeoAPI.Geometries;
using RealEstate.Domain.Tables;
using RealEstate.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace RealEstate.Services.Connector
{
    public interface IMapService
    {
        List<ApplicantViewModel> Map(ICollection<Applicant> model);

        List<UserViewModel> Map(ICollection<User> model);

        PropertyCategoryViewModel Map(UserPropertyCategory model);

        UserViewModel Map(User model);

        List<PropertyCategoryViewModel> Map(ICollection<UserPropertyCategory> model);

        PropertyOwnershipViewModel Map(PropertyOwnership model);

        PropertyViewModel Map(Property model);

        List<PropertyOwnershipViewModel> Map(ICollection<PropertyOwnership> model);

        DealPaymentViewModel Map(DealPayment model);

        List<BeneficiaryViewModel> Map(ICollection<Beneficiary> model);

        BeneficiaryViewModel Map(Beneficiary model);

        List<DealPaymentViewModel> Map(ICollection<DealPayment> model);

        DealViewModel Map(Deal model);

        List<ItemCategoryViewModel> Map(ICollection<UserItemCategory> model);

        DistrictViewModel Map(District model);

        GeolocationViewModel Map(IPoint model);

        ItemCategoryViewModel Map(UserItemCategory model);

        List<ItemRequestViewModel> Map(ICollection<ItemRequest> model);

        List<ItemViewModel> Map(ICollection<Item> model);

        ItemRequestViewModel Map(ItemRequest model);

        PropertyCategoryViewModel Map(PropertyCategory model);

        ItemViewModel Map(Item model);

        ItemCategoryViewModel Map(ItemCategory model);

        List<FacilityViewModel> Map(ICollection<PropertyFacility> model);

        List<FeatureValueViewModel> Map(ICollection<PropertyFeature> model);

        FeatureValueViewModel Map(ApplicantFeature model);

        List<FeatureValueViewModel> Map(ICollection<ApplicantFeature> model);

        List<FeatureValueViewModel> Map(ICollection<ItemFeature> model);

        FeatureValueViewModel Map(ItemFeature model);

        FeatureViewModel Map(Feature model);

        FeatureValueViewModel Map(PropertyFeature model);

        FacilityViewModel Map(PropertyFacility model);

        List<PictureViewModel> Map(ICollection<Picture> model);

        PictureViewModel Map(Picture model);

        OwnershipViewModel Map(Ownership model);

        ApplicantViewModel Map(Applicant model);

        List<OwnershipViewModel> Map(ICollection<Ownership> model);

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

        public List<ApplicantViewModel> Map(ICollection<Applicant> model)
        {
            if (model?.Any() != true)
                return default;

            var list = model.ToList();
            var result = _baseService.Map(list, Map);
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

        public List<PictureViewModel> Map(ICollection<Picture> model)
        {
            if (model?.Any() != true)
                return default;

            var list = model.ToList();
            var result = _baseService.Map(list, Map);
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
                    DealPayments = Map(model.DealPayments),
                    Beneficiaries = Map(model.Beneficiaries),
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
                    Features = Map(model.ApplicantFeatures),
                    Type = model.Type
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

        public List<OwnershipViewModel> Map(ICollection<Ownership> model)
        {
            if (model?.Any() != true)
                return default;

            var list = model.ToList();
            var result = _baseService.Map(list, Map);
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
                    Phone = model.PhoneNumber
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

        public List<FacilityViewModel> Map(ICollection<PropertyFacility> model)
        {
            if (model?.Any() != true)
                return default;

            var list = model.ToList();
            var result = _baseService.Map(list, Map);
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

        public List<FeatureValueViewModel> Map(ICollection<ApplicantFeature> model)
        {
            if (model?.Any() != true)
                return default;

            var list = model.ToList();
            var result = _baseService.Map(list, Map);
            return result;
        }

        public List<FeatureValueViewModel> Map(ICollection<ItemFeature> model)
        {
            if (model?.Any() != true)
                return default;

            var list = model.ToList();
            var result = _baseService.Map(list, Map);
            return result;
        }

        public List<FeatureValueViewModel> Map(ICollection<PropertyFeature> model)
        {
            if (model?.Any() != true)
                return default;

            var list = model.ToList();
            var result = _baseService.Map(list, Map);
            return result;
        }

        public ItemCategoryViewModel Map(ItemCategory model)
        {
            if (model == null)
                return null;

            var result = _baseService.Map(model,
                new ItemCategoryViewModel
                {
                    Id = model.Id,
                    Name = model.Name
                });
            return result;
        }

        public ItemCategoryViewModel Map(UserItemCategory model)
        {
            if (model == null)
                return null;

            var result = _baseService.Map(model, Map(model.ItemCategory));
            return result;
        }

        public ItemRequestViewModel Map(ItemRequest model)
        {
            if (model == null)
                return default;

            var result = _baseService.Map(model,
                new ItemRequestViewModel
                {
                    Applicants = Map(model.Applicants),
                    Item = Map(model.Item),
                    IsReject = model.IsReject
                });
            return result;
        }

        public List<ItemRequestViewModel> Map(ICollection<ItemRequest> model)
        {
            if (model?.Any() != true)
                return default;

            var list = model.ToList();
            var result = _baseService.Map(list, Map);
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
                    Features = Map(model.ItemFeatures),
                    IsRequested = model.ItemRequests.OrderByDescending(x => x.DateTime).FirstOrDefault()?.IsReject != true,
                    Property = Map(model.Property)
                });
            return result;
        }

        public List<ItemViewModel> Map(ICollection<Item> model)
        {
            if (model?.Any() != true)
                return default;

            var list = model.ToList();
            var result = _baseService.Map(list, Map);
            return result;
        }

        public List<ItemCategoryViewModel> Map(ICollection<UserItemCategory> model)
        {
            if (model?.Any() != true)
                return default;

            var list = model.ToList();
            var result = _baseService.Map(list, Map);
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
                    Commission = model.CommissionPrice,
                    PayDate = model.PayDate,
                    Pictures = Map(model.Pictures),
                    Text = model.Text,
                    Tip = model.TipPrice
                });
            return result;
        }

        public List<BeneficiaryViewModel> Map(ICollection<Beneficiary> model)
        {
            if (model?.Any() != true)
                return default;

            var list = model.ToList();
            var result = _baseService.Map(list, Map);
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

        public List<DealPaymentViewModel> Map(ICollection<DealPayment> model)
        {
            if (model?.Any() != true)
                return default;

            var list = model.ToList();
            var result = _baseService.Map(list, Map);
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
                    Category = Map(model.PropertyCategory),
                    Description = model.Description,
                    Deals = model.Items.Sum(x => x.ItemRequests.Count(c => c.Deal != null)),
                    District = Map(model.District),
                    Facilities = Map(model.PropertyFacilities),
                    Features = Map(model.PropertyFeatures),
                    Ownerships = Map(model.PropertyOwnerships),
                    Pictures = Map(model.Pictures),
                    Items = Map(model.Items),
                    Geolocation = Map(model.Geolocation)
                });
            return result;
        }

        public List<PropertyCategoryViewModel> Map(ICollection<UserPropertyCategory> model)
        {
            if (model?.Any() != true)
                return default;

            var list = model.ToList();
            var result = _baseService.Map(list, Map);
            return result;
        }

        public PropertyCategoryViewModel Map(UserPropertyCategory model)
        {
            if (model == null)
                return null;

            var result = _baseService.Map(model, Map(model.PropertyCategory));
            return result;
        }

        public List<PropertyOwnershipViewModel> Map(ICollection<PropertyOwnership> model)
        {
            if (model?.Any() != true)
                return default;

            var list = model.ToList();
            var result = _baseService.Map(list, Map);
            return result;
        }

        public PropertyOwnershipViewModel Map(PropertyOwnership model)
        {
            if (model == null)
                return default;

            var result = _baseService.Map(model,
                new PropertyOwnershipViewModel
                {
                    Owners = Map(model.Ownerships),
                });

            return result;
        }

        public PropertyCategoryViewModel Map(PropertyCategory model)
        {
            if (model == null)
                return default;

            var result = _baseService.Map(model,
                new PropertyCategoryViewModel
                {
                    Id = model.Id,
                    Name = model.Name
                });

            return result;
        }

        public List<UserViewModel> Map(ICollection<User> model)
        {
            if (model?.Any() != true)
                return default;

            var list = model.ToList();
            var result = _baseService.Map(list, Map);
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
                    ItemCategories = Map(model.UserItemCategories),
                    PropertyCategories = Map(model.UserPropertyCategories)
                });
            return result;
        }
    }
}