using Microsoft.EntityFrameworkCore;
using RealEstate.Base.Enums;
using RealEstate.Domain;
using RealEstate.Domain.Tables;
using RealEstate.Services.Connector;
using RealEstate.ViewModels;
using RealEstate.ViewModels.Input;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Services
{
    public interface IPropertyService
    {
        Task<(StatusEnum, Property)> PropertyAddAsync(PropertyInputViewModel model, bool save, UserViewModel user);
    }

    public class PropertyService : IPropertyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseService _baseService;
        private readonly IContactService _contactService;
        private readonly DbSet<Property> _properties;
        private readonly DbSet<Log> _logs;

        public PropertyService(
            IUnitOfWork unitOfWork,
            IBaseService baseService,
            IContactService contactService
            )
        {
            _unitOfWork = unitOfWork;
            _baseService = baseService;
            _contactService = contactService;
            _logs = _unitOfWork.PlugIn<Log>();
            _properties = _unitOfWork.PlugIn<Property>();
        }

        public async Task<(StatusEnum, Property)> PropertyAddAsync(PropertyInputViewModel model, bool save, UserViewModel user)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Property>(StatusEnum.ModelIsNull, null);

            if (model.Owners?.Any() != true)
                return new ValueTuple<StatusEnum, Property>(StatusEnum.OwnerIsNull, null);

            user = _baseService.CurrentUser(user);
            if (user == null)
                return new ValueTuple<StatusEnum, Property>(StatusEnum.UserIsNull, null);

            var newProperty = _unitOfWork.Add(new Property
            {
                Description = model.Description,
                DistrictId = model.DistrictId,
                PropertyCategoryId = model.CategoryId,
                Alley = model.Alley,
                BuildingName = model.BuildingName,
                Flat = model.Flat,
                Floor = model.Floor,
                Number = model.Number,
                Street = model.Street,
            }, user.Id);

            var (ownershipAddStatus, ownership) = await _contactService.OwnershipAddAsync(newProperty.Id, false, user).ConfigureAwait(false);
            if (ownershipAddStatus != StatusEnum.Success)
                return new ValueTuple<StatusEnum, Property>(StatusEnum.OwnershipIsNull, null);

            foreach (var owner in model.Owners)
                await _contactService.OwnerUpdateAsync(owner.OwnerId, ownership.Id, false, user).ConfigureAwait(false);

            await _baseService.SyncAsync(
                newProperty.PropertyFeatures,
                model.PropertyFeatures,
                (currentFeature, newFeature) => currentFeature.FeatureId == newFeature.Id,
                feature => new PropertyFeature
                {
                    PropertyId = newProperty.Id,
                    FeatureId = feature.Id,
                    Value = feature.Value,
                },
                false, user).ConfigureAwait(false);

            await _baseService.SyncAsync(
                newProperty.PropertyFacilities,
                model.PropertyFacilities,
                (currentFacility, newFacility) => currentFacility.FacilityId == newFacility.Id,
                facility => new PropertyFacility
                {
                    PropertyId = newProperty.Id,
                    FacilityId = facility.Id
                },
                false, user).ConfigureAwait(false);

            return await _baseService.SaveChangesAsync(newProperty, save).ConfigureAwait(false);
        }
    }
}